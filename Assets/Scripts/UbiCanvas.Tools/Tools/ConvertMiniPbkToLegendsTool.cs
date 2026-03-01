using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Animation;

namespace UbiCanvas.Tools {
	public class ConvertMiniPbkToLegendsTool : GameTool {
		public override string Name => "Convert PBK (Mini/Adventures/Fiesta Run iOS -> Legends)";
		public override string Description => "Copies AnimTemplates from a source .pbk.ckd (Rayman Origins, Mini, Adventures or Fiesta Run iOS) into a Rayman Legends .pbk.ckd while preserving Legends UInt32 identifiers for compatibility.";

		public enum PbkGameLocation {
			RaymanLegends,
			RaymanOrigins,
			RaymanMini,
			RaymanAdventures,
			RaymanFiestaRuniOS,
		}

		public PbkGameLocation MiniGame { get; set; } = PbkGameLocation.RaymanMini;
		public PbkGameLocation LegendsGame { get; set; } = PbkGameLocation.RaymanLegends;
		public PbkGameLocation OutputGame { get; set; } = PbkGameLocation.RaymanLegends;

		public string MiniPBKPath { get; set; }
		public string LegendsPBKPath { get; set; }
		public string OutputPBKPath { get; set; }

		public int ExcludedTemplateKeyCount { get; set; }
		public List<string> ExcludedTemplateKeyInputs { get; set; } = new();

		public bool ExcludeUntranslatableKeys { get; set; }

		public bool CopyOnlyPresetRaymanKeyNames { get; set; }

		public bool ForceAddMissingKeysFromSource { get; set; }

		public bool KeepOriginalTags { get; set; }

		public bool ReplaceAllLegendsKeysWithSource { get; set; }

		public Task ConvertAsync() {
			if (string.IsNullOrWhiteSpace(MiniPBKPath) || string.IsNullOrWhiteSpace(LegendsPBKPath) || string.IsNullOrWhiteSpace(OutputPBKPath)) {
				throw new InvalidOperationException("MiniPBKPath, LegendsPBKPath and OutputPBKPath are required");
			}

			string miniFullPath = ResolveFilePath(MiniGame, MiniPBKPath);
			string legendsFullPath = ResolveFilePath(LegendsGame, LegendsPBKPath);
			string outputFullPath = ResolveFilePath(OutputGame, OutputPBKPath);

			byte[] miniData = File.ReadAllBytes(miniFullPath);
			byte[] legendsData = File.ReadAllBytes(legendsFullPath);

			using Context miniContext = CreateContext(
				basePath: ResolveBundleDirectory(MiniGame),
				mode: GetModeForLocation(MiniGame),
				enableSerializerLog: false,
				loadAnimations: false,
				loadAllPaths: false);
			using Context legendsContext = CreateContext(
				basePath: ResolveBundleDirectory(LegendsGame),
				mode: GetModeForLocation(LegendsGame),
				enableSerializerLog: false,
				loadAnimations: false,
				loadAllPaths: false,
				loadStrings: ExcludeUntranslatableKeys);

			AnimPatchBank mini = CSerializable.CreateFromBinaryData(miniData, typeof(AnimPatchBank), "pbk", miniContext, allowLoadingReferences: true) as AnimPatchBank;
			AnimPatchBank legends = CSerializable.CreateFromBinaryData(legendsData, typeof(AnimPatchBank), "pbk", legendsContext, allowLoadingReferences: true) as AnimPatchBank;

			if (mini == null || legends == null) {
				throw new InvalidOperationException("Failed to deserialize one of the PBK files");
			}

			if (ReplaceAllLegendsKeysWithSource) {
				ReplaceAllTemplatesAndKeysFromSource(mini, legends);
			} else {
				HashSet<ulong> excludedKeys = ParseKeys(ExcludedTemplateKeyInputs);
				if (ExcludeUntranslatableKeys) {
					AddUntranslatableKeysToExcluded(excludedKeys, legends, legendsContext);
				}

				HashSet<ulong> onlyKeys = CopyOnlyPresetRaymanKeyNames ? GetPresetRaymanKeySet() : null;
				MergeTemplatesPreservingUInt32(mini, legends, excludedKeys, onlyKeys, ForceAddMissingKeysFromSource, KeepOriginalTags);
			}

			byte[] outputData = legends.CloneGetBinaryData("pbk", legendsContext, allowLoadingReferences: true);
			string outputDir = System.IO.Path.GetDirectoryName(outputFullPath);
			if (!string.IsNullOrWhiteSpace(outputDir)) {
				Directory.CreateDirectory(outputDir);
			}
			File.WriteAllBytes(outputFullPath, outputData);

			return Task.CompletedTask;
		}

		private static string ResolveFilePath(PbkGameLocation gameLocation, string path) {
			if (System.IO.Path.IsPathRooted(path)) {
				return path;
			}

			string bundleDir = ResolveBundleDirectory(gameLocation);
			string sanitized = path.Replace('\\', '/').TrimStart('/');

			string[] removablePrefixes = {
				"Bundle_PC/",
				"bundle_pc/",
				"Bundle_iOS/",
				"bundle_ios/",
				"bundle_android/",
				"bundle_macos/",
				"cache/itf_cooked/pc/",
				"cache/itf_cooked/ios/",
				"cache/itf_cooked/android/",
				"cache/itf_cooked/macos/",
				"itf_cooked/pc/",
				"itf_cooked/ios/",
				"itf_cooked/android/",
				"itf_cooked/macos/",
				"pc/",
				"ios/",
				"android/",
				"macos/",
			};
			foreach (string prefix in removablePrefixes) {
				if (sanitized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
					sanitized = sanitized.Substring(prefix.Length);
				}
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
				if (Directory.Exists(cookedDir)) {
					return cookedDir;
				}
			}

			// Legacy fallback if a project points directly to a Bundle_PC layout
			string bundleDir = System.IO.Path.Combine(normalizedGameDir, "Bundle_PC");
			if (Directory.Exists(bundleDir)) {
				return bundleDir;
			}

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

		private static readonly string[] PresetRaymanKeyNames = {
			"P_Ray_Hand02_L", "P_Ray_Hand09_R", "P_Ray_Hand01bis_L", "P_Ray_Eyes02_01", "P_Ray_Hand01_L", "P_Ray_Hand03_R",
			"P_Ray_Hand04_R", "P_Ray_Hand02bis_R", "P_Ray_Helico_03", "P_Ray_Hand30_L", "P_Ray_Hand05_R", "P_Ray_Helico_01",
			"P_Ray_Eyes03_01", "P_Ray_Eyes02_03", "P_Ray_Hand06_R", "P_Ray_Hand05bis_L", "P_Ray_Helico_02", "P_Ray_Eyes01_04",
			"P_Ray_Eyes01_03", "P_Ray_Hood01", "P_Ray_Foot01bis_L", "P_Ray_Hair01", "P_Ray_Mouth02", "P_Ray_Foot04bis_R",
			"P_Ray_Hair02", "P_Ray_Shadow", "P_Ray_Eyes01_05", "P_Ray_Hand19_R", "P_Ray_Eyes01_01", "P_Ray_Mouth01",
			"P_Ray_Foot02bis_L", "P_Ray_Gogle", "P_Ray_Foot01_L", "P_Ray_Foot03_R", "P_Ray_Hand09_L", "P_Ray_Body01",
			"P_Ray_Foot04_L", "P_Ray_Eyes02_02", "P_Ray_Foot04bis_L", "P_Ray_Hand31_R", "P_Ray_Hand19_L", "P_Ray_Foot01bis_R",
			"P_Ray_Hand07_L", "P_Ray_HoodStrings01", "P_Ray_Hand06_L", "P_Ray_Head_Helico", "P_Ray_Hand01bis_R", "P_Ray_Tongue01",
			"P_Ray_Hand03bis_L", "P_Ray_Hand02bis_L", "P_Ray_Hand18_L", "P_Ray_Hand01_R", "P_Ray_Foot03_L", "P_Ray_Head02",
			"P_Ray_Hand08_R", "P_Ray_Hand18_R", "P_Ray_Foot02_R", "P_Ray_Head01", "P_Ray_FX_Hand_a", "P_Ray_Foot04_R",
			"P_Ray_Hand04_L", "P_Ray_Hand08_L", "P_Ray_Foot01_R", "P_Ray_Hand07bis_R", "P_Ray_Hand02_R",
			"P_Ray_Hand17bis_R", "P_Ray_Body16", "P_Ray_Mouth07_02", "P_Ray_Hand16_L", "P_Ray_Eyes07_01", "P_Ray_Foot16_R", "P_Ray_Eyes09_02", "P_Ray_Eyes09_01", "P_Ray_Body17", "P_Ray_Hand17_R", "P_Ray_Hair03", "P_Ray_Hair09", "P_Ray_Head10", "P_Ray_Eyes07_02", "P_Ray_Foot16bis_R", "P_Ray_Body10", "P_Ray_Hand17_L", "P_Ray_Hand26_R", "P_Ray_Mouth07_01", "P_Ray_Hand16_R", "P_Ray_Head09", "P_Ray_Hand29_R", "P_Ray_Head07", "P_Ray_Head11", "P_Ray_Hand26_L", "P_Ray_Eyes10_01", "P_Ray_Hand29_L", "P_Ray_Circle", "P_Ray_Hand28_L", "P_Ray_Hand28_R", "P_Ray_Foot16_L",
			"P_Ray_Foot12s_L", "P_Ray_FX_Foot06_a", "P_Ray_Head12", "P_Ray_Foot06_R", "P_Ray_Foot06_L", "P_Ray_FX_Foot05_b", "P_Ray_Hand11_R", "P_Ray_Foot07_L", "P_Ray_Hand12_L", "P_Ray_Foot05_L", "P_Ray_Hair04_L", "P_Ray_Hand12_R", "P_Ray_Head03", "P_Ray_Foot12_L", "P_Ray_Tongue04", "P_Ray_Hand10_L", "P_Ray_Body12", "P_Ray_Hand11_L", "P_Ray_Eyes03", "P_Ray_Hand10_R", "P_Ray_Foot12_R", "P_Ray_Foot12s_R", "P_Ray_Foot07_R", "P_Ray_Hair04_R_02", "P_Ray_Hair04_R_01", "P_Ray_FX_Foot05_a", "P_Ray_Hood20", "P_Ray_Foot05_R", "P_Ray_Eyes04", "P_Ray_Body_03", "P_Ray_Head04", "P_Ray_Hand14_R",
			"P_Ray_Head20", "P_Ray_Eyes20_03", "P_Ray_Head18", "P_Ray_Head13", "P_Ray_Head15", "P_Ray_Foot13_R", "P_Ray_Foot20_R", "P_Ray_Eyes14", "P_Ray_Body20", "P_Ray_Eyes20_02", "P_Ray_Tongue15", "P_Ray_Hair20", "P_Ray_Tongue14", "P_Ray_Body15", "P_Ray_Body13", "P_Ray_Head14", "P_Ray_Foot20_L", "P_Ray_Foot13_L", "P_Ray_Hand30_R", "P_Ray_Hair18", "P_Ray_Splash", "P_Ray_Hand13_L", "P_Ray_Hair_15", "P_Ray_Eyes20_01", "P_Ray_Hair14",
			"P_Ray_FX_Punch_Charge_02", "P_Ray_FX_Punch_01", "P_Ray_FX_Kick_01", "P_Ray_FX_Kick_02"
		};

		private static readonly ulong[] PresetRaymanKeyIds = {
			0x090D8103, 0x09CB8858, 0x101497B5, 0x13CF2DF6, 0x17202527, 0x28B03DF7, 0x2D021BE2, 0x35580006,
			0x3B6A29D6, 0x4C044E64, 0x4E324483, 0x4EF916DA, 0x50CF43A4, 0x5650872F, 0x5CBF6A62, 0x7327F7D9,
			0x75CBAE68, 0x780B6967, 0x785446A6, 0x78BBF8E6, 0x7A7A11AF, 0x830ED184, 0x8A0F63B8, 0x8E36B905,
			0x8FCAEC41, 0x9C6E89C8, 0x9EE39B62, 0xA2AB177A, 0xA5453CE8, 0xB2AB1E75, 0xCAFA7688, 0xCBA57EED,
			0xDA58D0AA, 0xDC54CD0D, 0xE7636F45, 0xEF52BF1E, 0xEF847272, 0xF58A27B6, 0xFC50A222
		};

		private static HashSet<ulong> GetPresetRaymanKeySet() {
			HashSet<ulong> keys = new HashSet<ulong>(PresetRaymanKeyNames.Select(n => (ulong)new StringID(n).stringID));
			foreach (ulong key in PresetRaymanKeyIds) keys.Add(key);
			return keys;
		}

		private static void AddUntranslatableKeysToExcluded(HashSet<ulong> excludedKeys, AnimPatchBank legends, Context legendsContext) {
			if (excludedKeys == null || legends?.templates == null || legends?.templateKeys == null || legendsContext?.StringCache == null)
				return;

			for (int i = 0; i < legends.templates.Count; i++) {
				ulong key = legends.templateKeys.GetKeyFromValue(i);
				if (key > uint.MaxValue) {
					excludedKeys.Add(key);
					continue;
				}

				StringID sid = new StringID((uint)key);
				if (!legendsContext.StringCache.TryGetValue(sid, out string value) || string.IsNullOrWhiteSpace(value)) {
					excludedKeys.Add(key);
				}
			}
		}

		private static void ReplaceAllTemplatesAndKeysFromSource(AnimPatchBank source, AnimPatchBank target) {
			if (source?.templates == null || source.templateKeys == null || target == null) {
				throw new InvalidOperationException("Cannot replace PBK templates: invalid source or target data");
			}

			List<AnimTemplate> originalTargetTemplates = target.templates?.ToList() ?? new List<AnimTemplate>();
			List<ulong> originalTargetKeys = new List<ulong>();
			if (target.templateKeys != null) {
				int originalKeyCount = Math.Min(target.templateKeys.values?.Count ?? 0, originalTargetTemplates.Count);
				for (int i = 0; i < originalKeyCount; i++) {
					originalTargetKeys.Add(target.templateKeys.GetKey(i).stringID);
				}
			}

			target.templates = new CListO<AnimTemplate>();
			target.templateKeys ??= new KeyArray<int>();
			target.templateKeys.keys = new CArrayP<ulong>();
			target.templateKeys.keysLegends = new CArrayO<StringID>();
			target.templateKeys.values = new CArray<int>();

			for (int i = 0; i < source.templates.Count; i++) {
				AnimTemplate sourceTemplate = source.templates[i];
				if (sourceTemplate == null) continue;

				AnimTemplate resultTemplate;
				if (i < originalTargetTemplates.Count && originalTargetTemplates[i] != null) {
					resultTemplate = (AnimTemplate)MergePreservingUInt32(originalTargetTemplates[i], sourceTemplate, typeof(AnimTemplate), keepOriginalTags: false);
				} else {
					resultTemplate = sourceTemplate.Clone("pbk", context: target.UbiArtContext, allowLoadingReferences: true) as AnimTemplate;
				}
				if (resultTemplate == null) continue;
				EnsureSafePatchTopology(resultTemplate);

				ulong key = i < originalTargetKeys.Count ? originalTargetKeys[i] : source.templateKeys.GetKey(i).stringID;
				int newIndex = target.templates.Count;
				target.templates.Add(resultTemplate);
				target.templateKeys.keys.Add(key);
				target.templateKeys.keysLegends.Add(new StringID((uint)key));
				target.templateKeys.values.Add(newIndex);
			}

			target.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: full replace mode copied {target.templates.Count} template(s) from source while preserving original Legends UInt32 where possible");
		}

		private static void MergeTemplatesPreservingUInt32(AnimPatchBank mini, AnimPatchBank legends, HashSet<ulong> excludedKeys, HashSet<ulong> onlyKeys, bool forceAddMissingKeysFromSource, bool keepOriginalTags) {
			Dictionary<ulong, int> miniByKey = new();
			for (int i = 0; i < mini.templates.Count; i++) {
				miniByKey[mini.templateKeys.GetKeyFromValue(i)] = i;
			}

			Dictionary<ulong, int> legendsByKey = new();
			for (int i = 0; i < legends.templates.Count; i++) {
				legendsByKey[legends.templateKeys.GetKeyFromValue(i)] = i;
			}

			int matched = 0;
			int added = 0;
			List<ulong> missingInMini = new();

			for (int li = 0; li < legends.templates.Count; li++) {
				ulong key = legends.templateKeys.GetKeyFromValue(li);
				if (onlyKeys != null && !onlyKeys.Contains(key)) continue;
				if (excludedKeys != null && excludedKeys.Contains(key)) continue;
				if (!miniByKey.TryGetValue(key, out int mi)) {
					missingInMini.Add(key);
					continue;
				}

				var sourceTemplate = mini.templates[mi];
				var targetTemplate = legends.templates[li];
				if (sourceTemplate == null || targetTemplate == null) continue;

				int originalPatchPointCount = targetTemplate.patchPoints?.Count ?? 0;
				int originalPatchesCount = targetTemplate.patches?.Count ?? 0;
				int[] originalPatchLinkCounts = GetOriginalPatchLinkCounts(targetTemplate);

				AnimTemplate mergedTemplate = (AnimTemplate)MergePreservingUInt32(targetTemplate, sourceTemplate, typeof(AnimTemplate), keepOriginalTags);
				if (keepOriginalTags && mergedTemplate != null) {
					ClampPatchCollectionsToOriginalCount(mergedTemplate, originalPatchPointCount, originalPatchesCount);
					NormalizePatchLinksToExistingPatchPoints(mergedTemplate, originalPatchLinkCounts);
					EnsureSafePatchTopology(mergedTemplate);
				}

				legends.templates[li] = mergedTemplate;
				matched++;
			}

			if (forceAddMissingKeysFromSource) {
				for (int mi = 0; mi < mini.templates.Count; mi++) {
					ulong key = mini.templateKeys.GetKeyFromValue(mi);
					if (onlyKeys != null && !onlyKeys.Contains(key)) continue;
					if (excludedKeys != null && excludedKeys.Contains(key)) continue;
					if (legendsByKey.ContainsKey(key)) continue;

					AnimTemplate sourceTemplate = mini.templates[mi];
					if (sourceTemplate == null) continue;
					AnimTemplate clonedTemplate = sourceTemplate.Clone("pbk", context: legends.UbiArtContext, allowLoadingReferences: true) as AnimTemplate;
					if (clonedTemplate == null) continue;

					int newIndex = legends.templates.Count;
					legends.templates.Add(clonedTemplate);
					legends.templateKeys.keys?.Add(key);
					legends.templateKeys.keysLegends?.Add(new StringID((uint)key));
					legends.templateKeys.values?.Add(newIndex);
					legendsByKey[key] = newIndex;
					added++;
				}
			}

			mini.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: matched {matched}/{legends.templates.Count} templates by key");
			if (forceAddMissingKeysFromSource) {
				legends.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: added {added} missing key(s) from source");
			}
			if (excludedKeys != null && excludedKeys.Count > 0) {
				legends.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: skipped {excludedKeys.Count} excluded template key(s)");
			}
			if (onlyKeys != null) {
				legends.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: copy-only filter active ({onlyKeys.Count} preset key name(s))");
			}
			if (keepOriginalTags) {
				legends.UbiArtContext?.SystemLogger?.LogInfo("PBK convert: preserving original tags");
			}
			if (missingInMini.Count > 0) {
				legends.UbiArtContext?.SystemLogger?.LogWarning($"PBK convert: {missingInMini.Count} Legends template keys were not found in Mini and were kept unchanged");
			}
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
				if (template.patchPoints[i]?.key != null) {
					validPatchPointKeys.Add(template.patchPoints[i].key.stringID);
				}
			}

			for (int i = 0; i < template.patches.Count; i++) {
				AnimPatch patch = template.patches[i];
				if (patch?.points == null) continue;

				int maxPoints = (originalPatchLinkCounts != null && i < originalPatchLinkCounts.Length) ? originalPatchLinkCounts[i] : patch.points.Length;

				List<Link> filtered = new List<Link>();
				for (int p = 0; p < patch.points.Length && filtered.Count < maxPoints; p++) {
					Link pointLink = patch.points[p];
					if (pointLink != null && validPatchPointKeys.Contains(pointLink.stringID)) {
						filtered.Add(pointLink);
					}
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
				if (template.patchPoints[i] == null) {
					template.patchPoints.RemoveAt(i);
				}
			}

			for (int i = template.patches.Count - 1; i >= 0; i--) {
				AnimPatch patch = template.patches[i];
				if (patch == null) {
					template.patches.RemoveAt(i);
					continue;
				}
				patch.points ??= Array.Empty<Link>();
				if (patch.points.Length > byte.MaxValue) {
					Array.Resize(ref patch.points, byte.MaxValue);
				}
				patch.numPoints = (byte)patch.points.Length;
			}
		}

		private static HashSet<ulong> ParseKeys(IEnumerable<string> rawKeys) {
			HashSet<ulong> keys = new();
			if (rawKeys == null) return keys;

			foreach (string partRaw in rawKeys) {
				if (string.IsNullOrWhiteSpace(partRaw)) continue;
				string part = partRaw.Trim();
				if (part.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
					if (ulong.TryParse(part.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong hexKey))
						keys.Add(hexKey);
				} else if (ulong.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong decKey)) {
					keys.Add(decKey);
				}
			}
			return keys;
		}

		private static bool FieldNameMatches(string fieldName, params string[] candidates) {
			if (string.IsNullOrEmpty(fieldName) || candidates == null) return false;
			for (int i = 0; i < candidates.Length; i++) {
				if (string.Equals(fieldName, candidates[i], StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}

		private static object MergePreservingUInt32(object target, object source, Type declaredType, bool keepOriginalTags, string fieldName = null) {
			if (source == null) {
				if (keepOriginalTags && FieldNameMatches(fieldName, "patches", "patchPoints", "pointLinks", "points")) {
					return target;
				}
				return null;
			}
			if (target == null) return source;

			Type type = declaredType ?? source.GetType();
			if (type == typeof(uint) || type == typeof(UInt32) || type == typeof(Link) || type == typeof(StringID)) {
				return target;
			}
			if (keepOriginalTags && FieldNameMatches(fieldName, "pointLinks", "points")) {
				return target;
			}

			if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)) {
				return source;
			}

			if (typeof(IList).IsAssignableFrom(type)) {
				IList sourceList = source as IList;
				IList targetList = target as IList;
				if (sourceList == null || targetList == null) return source;

				Type elementType = typeof(object);
				if (type.IsArray) {
					elementType = type.GetElementType() ?? typeof(object);
				} else if (type.IsGenericType) {
					elementType = type.GetGenericArguments()[0];
				}

				if (keepOriginalTags && FieldNameMatches(fieldName, "patches")) {
					return target;
				}

				bool limitToTargetCount = keepOriginalTags && FieldNameMatches(fieldName, "patchPoints");
				int count = limitToTargetCount ? Math.Min(sourceList.Count, targetList.Count) : sourceList.Count;

				for (int i = 0; i < count; i++) {
					object sourceItem = sourceList[i];
					object targetItem = i < targetList.Count ? targetList[i] : null;
					object mergedItem = MergePreservingUInt32(targetItem, sourceItem, elementType, keepOriginalTags, fieldName);
					if (limitToTargetCount) {
						targetList[i] = mergedItem;
					} else {
						sourceList[i] = mergedItem;
					}
				}

				return limitToTargetCount ? target : source;
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {
				if (field.IsInitOnly || field.IsLiteral || field.IsStatic)
					continue;

				object sourceValue = field.GetValue(source);
				object targetValue = field.GetValue(target);
				object mergedValue = MergePreservingUInt32(targetValue, sourceValue, field.FieldType, keepOriginalTags, field.Name);
				field.SetValue(source, mergedValue);
			}

			return source;
		}
	}
}
