using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Animation;

namespace UbiCanvas.Tools {
	public class ConvertMiniPbkToLegendsTool : GameTool {
		public override string Name => "Convert PBK (Rayman Mini -> Rayman Legends)";
		public override string Description => "Copies AnimTemplates from a Rayman Mini .pbk.ckd into a Rayman Legends .pbk.ckd while preserving Legends UInt32 identifiers for compatibility.";

		public enum PbkGameLocation {
			RaymanLegends,
			RaymanMini,
			RaymanAdventures,
		}

		public PbkGameLocation MiniGame { get; set; } = PbkGameLocation.RaymanMini;
		public PbkGameLocation LegendsGame { get; set; } = PbkGameLocation.RaymanLegends;
		public PbkGameLocation OutputGame { get; set; } = PbkGameLocation.RaymanLegends;

		public string MiniPBKPath { get; set; }
		public string LegendsPBKPath { get; set; }
		public string OutputPBKPath { get; set; }

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
				loadAllPaths: false);

			AnimPatchBank mini = CSerializable.CreateFromBinaryData(miniData, typeof(AnimPatchBank), "pbk", miniContext, allowLoadingReferences: true) as AnimPatchBank;
			AnimPatchBank legends = CSerializable.CreateFromBinaryData(legendsData, typeof(AnimPatchBank), "pbk", legendsContext, allowLoadingReferences: true) as AnimPatchBank;

			if (mini == null || legends == null) {
				throw new InvalidOperationException("Failed to deserialize one of the PBK files");
			}

			MergeTemplatesPreservingUInt32(mini, legends);

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
				"cache/itf_cooked/pc/",
				"itf_cooked/pc/",
				"pc/",
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

			string bundleDir = System.IO.Path.Combine(gameDir, "Bundle_PC");
			if (Directory.Exists(bundleDir)) {
				return bundleDir;
			}

			return gameDir;
		}

		private static Mode GetModeForLocation(PbkGameLocation gameLocation) {
			return gameLocation switch {
				PbkGameLocation.RaymanLegends => Mode.RaymanLegendsPC,
				PbkGameLocation.RaymanMini => Mode.RaymanMiniMacOS,
				PbkGameLocation.RaymanAdventures => Mode.RaymanAdventuresAndroid,
				_ => throw new ArgumentOutOfRangeException(nameof(gameLocation), gameLocation, "Unsupported game location"),
			};
		}

		private static void MergeTemplatesPreservingUInt32(AnimPatchBank mini, AnimPatchBank legends) {
			Dictionary<ulong, int> miniByKey = new();
			for (int i = 0; i < mini.templates.Count; i++) {
				miniByKey[mini.templateKeys.GetKeyFromValue(i)] = i;
			}

			int matched = 0;
			List<ulong> missingInMini = new();

			for (int li = 0; li < legends.templates.Count; li++) {
				ulong key = legends.templateKeys.GetKeyFromValue(li);
				if (!miniByKey.TryGetValue(key, out int mi)) {
					missingInMini.Add(key);
					continue;
				}

				var sourceTemplate = mini.templates[mi];
				var targetTemplate = legends.templates[li];
				if (sourceTemplate == null || targetTemplate == null) {
					continue;
				}

				legends.templates[li] = (AnimTemplate)MergePreservingUInt32(targetTemplate, sourceTemplate, typeof(AnimTemplate));
				matched++;
			}

			mini.UbiArtContext?.SystemLogger?.LogInfo($"PBK convert: matched {matched}/{legends.templates.Count} templates by key");
			if (missingInMini.Count > 0) {
				legends.UbiArtContext?.SystemLogger?.LogWarning($"PBK convert: {missingInMini.Count} Legends template keys were not found in Mini and were kept unchanged");
			}
		}

		private static object MergePreservingUInt32(object target, object source, Type declaredType) {
			if (source == null) return null;
			if (target == null) return source;

			Type type = declaredType ?? source.GetType();
			if (type == typeof(uint) || type == typeof(UInt32) || type == typeof(Link) || type == typeof(StringID)) {
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

				for (int i = 0; i < sourceList.Count; i++) {
					object sourceItem = sourceList[i];
					object targetItem = i < targetList.Count ? targetList[i] : null;
					sourceList[i] = MergePreservingUInt32(targetItem, sourceItem, elementType);
				}

				return source;
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {
				if (field.IsInitOnly || field.IsLiteral || field.IsStatic)
					continue;

				object sourceValue = field.GetValue(source);
				object targetValue = field.GetValue(target);
				object mergedValue = MergePreservingUInt32(targetValue, sourceValue, field.FieldType);
				field.SetValue(source, mergedValue);
			}

			return source;
		}
	}
}
