using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using UbiArt;
using UbiArt.Animation;

namespace UbiCanvas.Tools {
	public class PbkAtlasTemplateTransferTool : GameTool {
		public override string Name => "PBK Atlas Template Transfer";
		public override string Description => "Select source/target texture+PBK, then copy all source templates to target PBK and save.";

		public enum PbkGameLocation {
			RaymanLegends,
			RaymanOrigins,
			RaymanMini,
			RaymanAdventures,
			RaymanFiestaRuniOS,
		}

		public PbkGameLocation SourceGame { get; set; } = PbkGameLocation.RaymanAdventures;
		public string SourceTexturePath { get; set; }
		public string SourcePBKPath { get; set; }

		public PbkGameLocation TargetGame { get; set; } = PbkGameLocation.RaymanLegends;
		public string TargetTexturePath { get; set; }
		public string TargetPBKPath { get; set; }

		public PbkGameLocation OutputGame { get; set; } = PbkGameLocation.RaymanLegends;
		public string OutputPBKPath { get; set; }

		public bool PreserveTargetUInt32Identifiers { get; set; } = true;

		public bool PreserveTargetPatchTopology { get; set; } = true;

		public bool PreserveTargetBoneDynamicsAndScale { get; set; } = true;

		public Task TransferAsync() {
			if (string.IsNullOrWhiteSpace(SourcePBKPath) || string.IsNullOrWhiteSpace(TargetPBKPath) || string.IsNullOrWhiteSpace(OutputPBKPath)) {
				throw new InvalidOperationException("SourcePBKPath, TargetPBKPath and OutputPBKPath are required");
			}

			string sourcePbkFull = ResolveFilePath(SourceGame, SourcePBKPath);
			string targetPbkFull = ResolveFilePath(TargetGame, TargetPBKPath);
			string outputPbkFull = ResolveFilePath(OutputGame, OutputPBKPath);

			using Context sourceContext = CreateContext(basePath: ResolveBundleDirectory(SourceGame), mode: GetModeForLocation(SourceGame), enableSerializerLog: false, loadAnimations: false, loadAllPaths: false);
			using Context targetContext = CreateContext(basePath: ResolveBundleDirectory(TargetGame), mode: GetModeForLocation(TargetGame), enableSerializerLog: false, loadAnimations: false, loadAllPaths: false);

			ValidateTexturePathIfProvided(SourceGame, SourceTexturePath, sourceContext, "source");
			ValidateTexturePathIfProvided(TargetGame, TargetTexturePath, targetContext, "target");

			byte[] sourceBytes = File.ReadAllBytes(sourcePbkFull);
			byte[] targetBytes = File.ReadAllBytes(targetPbkFull);

			AnimPatchBank sourcePbk = CSerializable.CreateFromBinaryData(sourceBytes, typeof(AnimPatchBank), "pbk", sourceContext, allowLoadingReferences: true) as AnimPatchBank;
			AnimPatchBank targetPbk = CSerializable.CreateFromBinaryData(targetBytes, typeof(AnimPatchBank), "pbk", targetContext, allowLoadingReferences: true) as AnimPatchBank;
			if (sourcePbk == null || targetPbk == null) {
				throw new InvalidOperationException("Failed to deserialize source/target PBK");
			}

			ReplaceAllTemplatesFromSource(sourcePbk, targetPbk, PreserveTargetUInt32Identifiers, PreserveTargetPatchTopology, PreserveTargetBoneDynamicsAndScale);

			byte[] outputData = targetPbk.CloneGetBinaryData("pbk", targetContext, allowLoadingReferences: true);
			string outputDir = System.IO.Path.GetDirectoryName(outputPbkFull);
			if (!string.IsNullOrWhiteSpace(outputDir)) Directory.CreateDirectory(outputDir);
			File.WriteAllBytes(outputPbkFull, outputData);

			targetContext.SystemLogger?.LogInfo($"PBK transfer: copied {targetPbk.templates?.Count ?? 0} template(s) from source to target and saved {outputPbkFull}");
			return Task.CompletedTask;
		}

		private static void ReplaceAllTemplatesFromSource(AnimPatchBank source, AnimPatchBank target, bool preserveTargetUInt32Identifiers, bool preserveTargetPatchTopology, bool preserveTargetBoneDynamicsAndScale) {
			if (source?.templates == null || source.templateKeys == null || target == null) {
				throw new InvalidOperationException("Invalid PBK data for template transfer");
			}

			List<AnimTemplate> originalTargetTemplates = target.templates?.ToList() ?? new List<AnimTemplate>();
			List<ulong> originalTargetKeys = new List<ulong>();
			if (target.templateKeys != null) {
				int originalCount = Math.Min(target.templateKeys.values?.Count ?? 0, originalTargetTemplates.Count);
				for (int i = 0; i < originalCount; i++) {
					originalTargetKeys.Add(target.templateKeys.GetKey(i).stringID);
				}
			}

			target.templates = new CListO<AnimTemplate>();
			target.templateKeys ??= new KeyArray<int>();
			target.templateKeys.keys = new CArrayP<ulong>();
			target.templateKeys.keysLegends = new CArrayO<StringID>();
			target.templateKeys.values = new CArray<int>();

			if (preserveTargetUInt32Identifiers) {
				Dictionary<ulong, AnimTemplate> sourceByKey = BuildTemplatesByKey(source);
				for (int i = 0; i < originalTargetTemplates.Count; i++) {
					AnimTemplate originalTargetTemplate = originalTargetTemplates[i];
					if (originalTargetTemplate == null) continue;

					ulong targetKey = i < originalTargetKeys.Count ? originalTargetKeys[i] : ulong.MaxValue;
					sourceByKey.TryGetValue(targetKey, out AnimTemplate sourceTemplate);

					AnimTemplate resultTemplate;
					if (sourceTemplate != null) {
						int originalPatchPointCount = originalTargetTemplate.patchPoints?.Count ?? 0;
						int originalPatchesCount = originalTargetTemplate.patches?.Count ?? 0;
						int[] originalPatchLinkCounts = GetOriginalPatchLinkCounts(originalTargetTemplate);

						resultTemplate = (AnimTemplate)MergePreservingUInt32(originalTargetTemplate, sourceTemplate, typeof(AnimTemplate), preserveTargetPatchTopology, preserveTargetBoneDynamicsAndScale);
						ClampPatchCollectionsToOriginalCount(resultTemplate, originalPatchPointCount, originalPatchesCount);
						NormalizePatchLinksToExistingPatchPoints(resultTemplate, originalPatchLinkCounts);
					} else {
						resultTemplate = originalTargetTemplate.Clone("pbk", context: target.UbiArtContext, allowLoadingReferences: true) as AnimTemplate;
					}

					if (resultTemplate == null) continue;
					EnsureSafePatchTopology(resultTemplate);

					int index = target.templates.Count;
					target.templates.Add(resultTemplate);
					target.templateKeys.keys.Add(targetKey == ulong.MaxValue ? 0 : targetKey);
					target.templateKeys.keysLegends.Add(new StringID((uint)(targetKey == ulong.MaxValue ? 0 : targetKey)));
					target.templateKeys.values.Add(index);
				}
			} else {
				for (int i = 0; i < source.templates.Count; i++) {
					AnimTemplate sourceTemplate = source.templates[i];
					if (sourceTemplate == null) continue;

					AnimTemplate resultTemplate = sourceTemplate.Clone("pbk", context: target.UbiArtContext, allowLoadingReferences: true) as AnimTemplate;
					if (resultTemplate == null) continue;
					EnsureSafePatchTopology(resultTemplate);

					ulong key = source.templateKeys.GetKey(i).stringID;
					int index = target.templates.Count;
					target.templates.Add(resultTemplate);
					target.templateKeys.keys.Add(key);
					target.templateKeys.keysLegends.Add(new StringID((uint)key));
					target.templateKeys.values.Add(index);
				}
			}
		}

		private static Dictionary<ulong, AnimTemplate> BuildTemplatesByKey(AnimPatchBank source) {
			Dictionary<ulong, AnimTemplate> byKey = new Dictionary<ulong, AnimTemplate>();
			if (source?.templates == null || source.templateKeys == null) return byKey;
			for (int i = 0; i < source.templates.Count; i++) {
				AnimTemplate t = source.templates[i];
				if (t == null) continue;
				ulong key = source.templateKeys.GetKey(i).stringID;
				if (!byKey.ContainsKey(key)) byKey[key] = t;
			}
			return byKey;
		}


		private static bool FieldNameMatches(string fieldName, params string[] candidates) {
			if (string.IsNullOrEmpty(fieldName) || candidates == null) return false;
			for (int i = 0; i < candidates.Length; i++) {
				if (string.Equals(fieldName, candidates[i], StringComparison.OrdinalIgnoreCase)) return true;
			}
			return false;
		}

		private static object MergePreservingUInt32(object target, object source, Type declaredType, bool preserveTargetPatchTopology, bool preserveTargetBoneDynamicsAndScale, string fieldName = null) {
			if (source == null) {
				if (preserveTargetPatchTopology && FieldNameMatches(fieldName, "patches", "patchPoints", "pointLinks", "points")) return target;
				if (preserveTargetBoneDynamicsAndScale && FieldNameMatches(fieldName, "bonesDyn", "SizeMultiplier")) return target;
				return null;
			}
			if (target == null) return source;
			Type type = declaredType ?? source.GetType();
			if (type == typeof(uint) || type == typeof(UInt32) || type == typeof(Link) || type == typeof(StringID)) return target;
			if (preserveTargetPatchTopology && FieldNameMatches(fieldName, "patches", "patchPoints", "pointLinks", "points")) return target;
			if (preserveTargetBoneDynamicsAndScale && FieldNameMatches(fieldName, "bonesDyn", "SizeMultiplier")) return target;
			if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)) return source;
			if (typeof(IList).IsAssignableFrom(type)) {
				IList sourceList = source as IList;
				IList targetList = target as IList;
				if (sourceList == null || targetList == null) return source;
				Type elementType = typeof(object);
				if (type.IsArray) elementType = type.GetElementType() ?? typeof(object);
				else if (type.IsGenericType) elementType = type.GetGenericArguments()[0];
				int count = Math.Min(sourceList.Count, targetList.Count);
				for (int i = 0; i < count; i++) {
					targetList[i] = MergePreservingUInt32(targetList[i], sourceList[i], elementType, preserveTargetPatchTopology, preserveTargetBoneDynamicsAndScale, fieldName);
				}
				return target;
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo f = fields[i];
				if (f.IsStatic || f.IsInitOnly || f.IsLiteral) continue;
				object merged = MergePreservingUInt32(f.GetValue(target), f.GetValue(source), f.FieldType, preserveTargetPatchTopology, preserveTargetBoneDynamicsAndScale, f.Name);
				f.SetValue(source, merged);
			}
			return source;
		}


		private static int[] GetOriginalPatchLinkCounts(AnimTemplate targetTemplate) {
			if (targetTemplate?.patches == null) return Array.Empty<int>();
			int[] counts = new int[targetTemplate.patches.Count];
			for (int i = 0; i < targetTemplate.patches.Count; i++) {
				counts[i] = targetTemplate.patches[i]?.points?.Length ?? 0;
			}
			return counts;
		}

		private static void ClampPatchCollectionsToOriginalCount(AnimTemplate template, int originalPatchPointCount, int originalPatchesCount) {
			if (template?.patchPoints != null) {
				while (template.patchPoints.Count > originalPatchPointCount)
					template.patchPoints.RemoveAt(template.patchPoints.Count - 1);
			}
			if (template?.patches != null) {
				while (template.patches.Count > originalPatchesCount)
					template.patches.RemoveAt(template.patches.Count - 1);
			}
		}

		private static void NormalizePatchLinksToExistingPatchPoints(AnimTemplate template, int[] originalPatchLinkCounts) {
			if (template?.patchPoints == null || template.patches == null) return;
			HashSet<uint> validPatchPointKeys = new HashSet<uint>();
			for (int i = 0; i < template.patchPoints.Count; i++) {
				if (template.patchPoints[i]?.key != null) validPatchPointKeys.Add(template.patchPoints[i].key.stringID);
			}
			for (int i = 0; i < template.patches.Count; i++) {
				AnimPatch patch = template.patches[i];
				if (patch?.points == null) continue;
				int maxPoints = (originalPatchLinkCounts != null && i < originalPatchLinkCounts.Length) ? originalPatchLinkCounts[i] : patch.points.Length;
				List<Link> filtered = new List<Link>();
				for (int p = 0; p < patch.points.Length && filtered.Count < maxPoints; p++) {
					Link pointLink = patch.points[p];
					if (pointLink != null && validPatchPointKeys.Contains(pointLink.stringID)) filtered.Add(pointLink);
				}
				patch.points = filtered.ToArray();
				patch.numPoints = (byte)patch.points.Length;
			}
		}

		private static void EnsureSafePatchTopology(AnimTemplate template) {
			if (template == null) return;
			template.patchPoints ??= new CListO<AnimPatchPoint>();
			template.patches ??= new CListO<AnimPatch>();
			for (int i = template.patchPoints.Count - 1; i >= 0; i--) {
				if (template.patchPoints[i] == null) template.patchPoints.RemoveAt(i);
			}
			for (int i = template.patches.Count - 1; i >= 0; i--) {
				AnimPatch patch = template.patches[i];
				if (patch == null) { template.patches.RemoveAt(i); continue; }
				patch.points ??= Array.Empty<Link>();
				if (patch.points.Length > byte.MaxValue) Array.Resize(ref patch.points, byte.MaxValue);
				patch.numPoints = (byte)patch.points.Length;
			}
		}

		private static void ValidateTexturePathIfProvided(PbkGameLocation game, string texturePath, Context context, string side) {
			if (string.IsNullOrWhiteSpace(texturePath)) return;
			string resolved = ResolveFilePath(game, texturePath);
			if (!File.Exists(resolved) && !(context?.Loader?.FileManager?.FileExists(texturePath) ?? false)) {
				throw new FileNotFoundException($"{side} texture file not found", resolved);
			}
		}

		private static string ResolveFilePath(PbkGameLocation gameLocation, string path) {
			if (System.IO.Path.IsPathRooted(path)) return path;
			string bundleDir = ResolveBundleDirectory(gameLocation);
			string sanitized = path.Replace('\\', '/').TrimStart('/');

			string[] removablePrefixes = {
				"Bundle_PC/", "bundle_pc/", "Bundle_iOS/", "bundle_ios/", "bundle_android/", "bundle_macos/",
				"cache/itf_cooked/pc/", "cache/itf_cooked/ios/", "cache/itf_cooked/android/", "cache/itf_cooked/macos/",
				"itf_cooked/pc/", "itf_cooked/ios/", "itf_cooked/android/", "itf_cooked/macos/", "pc/", "ios/", "android/", "macos/"
			};
			for (int i = 0; i < removablePrefixes.Length; i++) {
				string prefix = removablePrefixes[i];
				if (sanitized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) sanitized = sanitized.Substring(prefix.Length);
			}
			return System.IO.Path.Combine(bundleDir, sanitized);
		}

		private static string ResolveBundleDirectory(PbkGameLocation gameLocation) {
			Mode mode = GetModeForLocation(gameLocation);
			if (!UnitySettings.GameDirs.TryGetValue(mode, out string gameDir) || string.IsNullOrWhiteSpace(gameDir)) {
				throw new InvalidOperationException($"Game directory is not configured for {gameLocation} ({mode}). Configure it in Settings first.");
			}

			string normalizedGameDir = gameDir.Replace('\\', '/').TrimEnd('/');
			string cookedRelative = Settings.FromMode(mode).ITFDirectory.Replace('\\', '/').Trim('/');
			if (!string.IsNullOrWhiteSpace(cookedRelative)) {
				string cookedDir = System.IO.Path.Combine(normalizedGameDir, cookedRelative);
				if (Directory.Exists(cookedDir)) return cookedDir;
			}

			string bundleDir = System.IO.Path.Combine(normalizedGameDir, "Bundle_PC");
			if (Directory.Exists(bundleDir)) return bundleDir;
			return normalizedGameDir;
		}

		private static Mode GetModeForLocation(PbkGameLocation gameLocation) {
			return gameLocation switch {
				PbkGameLocation.RaymanLegends => Mode.RaymanLegendsPC,
				PbkGameLocation.RaymanOrigins => Mode.RaymanOriginsPC,
				PbkGameLocation.RaymanMini => Mode.RaymanMiniMacOS,
				PbkGameLocation.RaymanAdventures => Mode.RaymanAdventuresAndroid,
				PbkGameLocation.RaymanFiestaRuniOS => Mode.RaymanFiestaRuniOS,
				_ => throw new ArgumentOutOfRangeException(nameof(gameLocation), gameLocation, "Unsupported game location"),
			};
		}
	}
}
