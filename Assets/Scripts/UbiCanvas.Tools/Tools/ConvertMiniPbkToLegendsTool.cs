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

		public string MiniPBKPath { get; set; }
		public string LegendsPBKPath { get; set; }
		public string OutputPBKPath { get; set; }

		public Task ConvertAsync() {
			if (string.IsNullOrWhiteSpace(MiniPBKPath) || string.IsNullOrWhiteSpace(LegendsPBKPath) || string.IsNullOrWhiteSpace(OutputPBKPath)) {
				throw new InvalidOperationException("MiniPBKPath, LegendsPBKPath and OutputPBKPath are required");
			}

			byte[] miniData = File.ReadAllBytes(MiniPBKPath);
			byte[] legendsData = File.ReadAllBytes(LegendsPBKPath);

			using Context miniContext = CreateContext(
				basePath: System.IO.Path.GetDirectoryName(MiniPBKPath),
				mode: Mode.RaymanMiniMacOS,
				enableSerializerLog: false,
				loadAnimations: false,
				loadAllPaths: false);
			using Context legendsContext = CreateContext(
				basePath: System.IO.Path.GetDirectoryName(LegendsPBKPath),
				mode: Mode.RaymanLegendsPC,
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
			string outputDir = System.IO.Path.GetDirectoryName(OutputPBKPath);
			if (!string.IsNullOrWhiteSpace(outputDir)) {
				Directory.CreateDirectory(outputDir);
			}
			File.WriteAllBytes(OutputPBKPath, outputData);

			return Task.CompletedTask;
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
