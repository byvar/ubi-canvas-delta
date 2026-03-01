using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Animation;
using UnityEngine;

namespace UbiCanvas.Tools {
	public class PbkDeepDiffTool : GameTool {
		public override string Name => "PBK Deep Diff";
		public override string Description => "Deserialize two PBKs and recursively compare all fields/lists to log complete differences for crash diagnostics.";

		public enum PbkGameLocation {
			RaymanLegends,
			RaymanOrigins,
			RaymanMini,
			RaymanAdventures,
			RaymanFiestaRuniOS,
		}

		public PbkGameLocation FirstGame { get; set; } = PbkGameLocation.RaymanLegends;
		public string FirstPBKPath { get; set; }
		public PbkGameLocation SecondGame { get; set; } = PbkGameLocation.RaymanAdventures;
		public string SecondPBKPath { get; set; }
		public int MaxDifferencesToLog { get; set; } = 3000;

		public Task CompareAsync() {
			if (string.IsNullOrWhiteSpace(FirstPBKPath) || string.IsNullOrWhiteSpace(SecondPBKPath)) {
				throw new InvalidOperationException("FirstPBKPath and SecondPBKPath are required");
			}

			string firstFullPath = ResolveFilePath(FirstGame, FirstPBKPath);
			string secondFullPath = ResolveFilePath(SecondGame, SecondPBKPath);

			byte[] firstData = File.ReadAllBytes(firstFullPath);
			byte[] secondData = File.ReadAllBytes(secondFullPath);

			using Context firstContext = CreateContext(basePath: ResolveBundleDirectory(FirstGame), mode: GetModeForLocation(FirstGame), enableSerializerLog: false, loadAnimations: false, loadAllPaths: false, loadStrings: true);
			using Context secondContext = CreateContext(basePath: ResolveBundleDirectory(SecondGame), mode: GetModeForLocation(SecondGame), enableSerializerLog: false, loadAnimations: false, loadAllPaths: false, loadStrings: true);

			AnimPatchBank first = CSerializable.CreateFromBinaryData(firstData, typeof(AnimPatchBank), "pbk", firstContext, allowLoadingReferences: true) as AnimPatchBank;
			AnimPatchBank second = CSerializable.CreateFromBinaryData(secondData, typeof(AnimPatchBank), "pbk", secondContext, allowLoadingReferences: true) as AnimPatchBank;
			if (first == null || second == null) {
				throw new InvalidOperationException("Failed to deserialize one of the PBK files");
			}

			DifferenceCollector collector = new DifferenceCollector(MaxDifferencesToLog);
			CompareObject("pbk", first, second, typeof(AnimPatchBank), collector, new HashSet<string>());

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"PBK deep diff between {FirstGame} and {SecondGame}");
			sb.AppendLine($"First: {FirstPBKPath}");
			sb.AppendLine($"Second: {SecondPBKPath}");
			sb.AppendLine($"Differences found: {collector.TotalDifferences}");
			if (collector.IsTruncated) sb.AppendLine($"(Truncated to first {collector.MaxDifferences} differences)");
			sb.AppendLine();

			for (int i = 0; i < collector.Differences.Count; i++) {
				sb.AppendLine($"[{i + 1}] {collector.Differences[i]}");
			}

			string report = sb.ToString();
			Debug.Log(report);
			firstContext.SystemLogger?.LogInfo(report);
			return Task.CompletedTask;
		}



		private static bool ShouldCompareUInt32(uint secondValue) {
			return secondValue < 10000;
		}

		private static bool IsTagFieldPath(string path) {
			if (string.IsNullOrEmpty(path)) return false;
			string lower = path.ToLowerInvariant();
			return lower.EndsWith(".tag") || lower.EndsWith(".tags") || lower.Contains(".tag[") || lower.Contains(".tags[");
		}

		private static bool TryParseNumericString(string value, out ulong parsed) {
			parsed = 0;
			if (string.IsNullOrWhiteSpace(value)) return false;
			string v = value.Trim();
			if (v.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
				return ulong.TryParse(v.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out parsed);
			}
			return ulong.TryParse(v, out parsed);
		}

		private static bool AreEquivalentNumericRepresentations(object first, object second) {
			if (first is string a && second is string b) {
				if (TryParseNumericString(a, out ulong pa) && TryParseNumericString(b, out ulong pb)) {
					return pa == pb;
				}
			}
			return false;
		}

		private static void CompareObject(string path, object first, object second, Type declaredType, DifferenceCollector collector, HashSet<string> visitedPairs) {
			if (collector.IsTruncated) return;
			if (IsTagFieldPath(path)) return;
			if (ReferenceEquals(first, second)) return;

			if (first == null || second == null) {
				collector.Add($"{path}: {(first == null ? "first" : "second")} is null");
				return;
			}

			Type type = declaredType ?? first.GetType();
			if (type != first.GetType() && !type.IsAssignableFrom(first.GetType())) type = first.GetType();

			if (first is AnimPatchBank firstPbk && second is AnimPatchBank secondPbk) {
				CompareAnimPatchBanks(path, firstPbk, secondPbk, collector, visitedPairs);
				return;
			}

			if (first is AnimTemplate firstTemplate && second is AnimTemplate secondTemplate) {
				CompareAnimTemplates(path, firstTemplate, secondTemplate, collector, visitedPairs);
				return;
			}

			if (type == typeof(uint) || type == typeof(UInt32)) {
				uint a = (uint)first;
				uint b = (uint)second;
				if (!ShouldCompareUInt32(b)) return;
				if (a != b) collector.Add($"{path}: UInt32 {a} != {b}");
				return;
			}

			if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)) {
				if (!Equals(first, second) && !AreEquivalentNumericRepresentations(first, second)) {
					collector.Add($"{path}: {first} != {second}");
				}
				return;
			}

			if (type == typeof(Link)) {
				uint a = ((Link)first).stringID;
				uint b = ((Link)second).stringID;
				if (!ShouldCompareUInt32(b)) return;
				if (a != b) collector.Add($"{path}: Link {a:X8} != {b:X8}");
				return;
			}
			if (type == typeof(StringID)) {
				uint a = ((StringID)first).stringID;
				uint b = ((StringID)second).stringID;
				if (!ShouldCompareUInt32(b)) return;
				if (a != b) collector.Add($"{path}: StringID {a:X8} != {b:X8}");
				return;
			}

			if (typeof(IList).IsAssignableFrom(type)) {
				IList listA = first as IList;
				IList listB = second as IList;
				if (listA == null || listB == null) {
					collector.Add($"{path}: list cast failed");
					return;
				}
				if (listA.Count != listB.Count) {
					collector.Add($"{path}: count {listA.Count} != {listB.Count}");
				}

				Type elemType = typeof(object);
				if (type.IsArray) elemType = type.GetElementType() ?? typeof(object);
				else if (type.IsGenericType) elemType = type.GetGenericArguments()[0];

				int min = Math.Min(listA.Count, listB.Count);
				for (int i = 0; i < min; i++) {
					CompareObject($"{path}[{i}]", listA[i], listB[i], elemType, collector, visitedPairs);
					if (collector.IsTruncated) return;
				}
				return;
			}

			string pairKey = $"{path}:{first.GetHashCode()}:{second.GetHashCode()}";
			if (!type.IsValueType) {
				if (visitedPairs.Contains(pairKey)) return;
				visitedPairs.Add(pairKey);
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo field = fields[i];
				if (field.IsStatic) continue;
				if (string.Equals(field.Name, "UbiArtContext", StringComparison.OrdinalIgnoreCase) || string.Equals(field.Name, "IsFirstLoad", StringComparison.OrdinalIgnoreCase)) continue;
				if (string.Equals(field.Name, "tag", StringComparison.OrdinalIgnoreCase) || string.Equals(field.Name, "tags", StringComparison.OrdinalIgnoreCase)) continue;
				object a = field.GetValue(first);
				object b = field.GetValue(second);
				CompareObject($"{path}.{field.Name}", a, b, field.FieldType, collector, visitedPairs);
				if (collector.IsTruncated) return;
			}
		}

		private static void CompareAnimPatchBanks(string path, AnimPatchBank first, AnimPatchBank second, DifferenceCollector collector, HashSet<string> visitedPairs) {
			CompareObject($"{path}.version", first.version, second.version, typeof(uint), collector, visitedPairs);
			CompareObject($"{path}.bankId", first.bankId, second.bankId, typeof(Link), collector, visitedPairs);
			CompareObject($"{path}.unk", first.unk, second.unk, typeof(float), collector, visitedPairs);

			Dictionary<ulong, AnimTemplate> firstByKey = BuildTemplatesByKey(first);
			Dictionary<ulong, AnimTemplate> secondByKey = BuildTemplatesByKey(second);

			foreach (ulong key in firstByKey.Keys) {
				if (!secondByKey.ContainsKey(key)) {
					collector.Add($"{path}.templatesByKey: missing in second key 0x{key:X8}");
					if (collector.IsTruncated) return;
				}
			}
			foreach (ulong key in secondByKey.Keys) {
				if (!firstByKey.ContainsKey(key)) {
					collector.Add($"{path}.templatesByKey: missing in first key 0x{key:X8}");
					if (collector.IsTruncated) return;
				}
			}

			foreach (ulong key in firstByKey.Keys) {
				if (!secondByKey.TryGetValue(key, out AnimTemplate secondTemplate)) continue;
				CompareObject($"{path}.templatesByKey[0x{key:X8}]", firstByKey[key], secondTemplate, typeof(AnimTemplate), collector, visitedPairs);
				if (collector.IsTruncated) return;
			}
		}

		private static Dictionary<ulong, AnimTemplate> BuildTemplatesByKey(AnimPatchBank pbk) {
			Dictionary<ulong, AnimTemplate> byKey = new Dictionary<ulong, AnimTemplate>();
			if (pbk?.templates == null || pbk.templateKeys == null) return byKey;
			for (int i = 0; i < pbk.templates.Count; i++) {
				ulong key = pbk.templateKeys.GetKeyFromValue(i);
				if (!byKey.ContainsKey(key)) {
					byKey[key] = pbk.templates[i];
				}
			}
			return byKey;
		}

		private static void CompareAnimTemplates(string path, AnimTemplate first, AnimTemplate second, DifferenceCollector collector, HashSet<string> visitedPairs) {
			CompareObject($"{path}.SizeMultiplier", first.SizeMultiplier, second.SizeMultiplier, typeof(float), collector, visitedPairs);
			CompareObject($"{path}.bonesDyn", first.bonesDyn, second.bonesDyn, typeof(CListO<AnimBoneDyn>), collector, visitedPairs);
			CompareObject($"{path}.patches", first.patches, second.patches, typeof(CListO<AnimPatch>), collector, visitedPairs);

			CompareListByKey(path + ".bonesByKey", first.bones, second.bones, b => b?.key?.stringID ?? uint.MaxValue, collector, visitedPairs, typeof(AnimBone));
			CompareListByKey(path + ".patchPointsByKey", first.patchPoints, second.patchPoints, p => p?.key?.stringID ?? uint.MaxValue, collector, visitedPairs, typeof(AnimPatchPoint));
		}

		private static bool IsLowRange(uint key) => key < 10000;

		private static bool HasDifferentKeyNamespace<T>(Dictionary<uint, T> firstByKey, Dictionary<uint, T> secondByKey) {
			if (firstByKey.Count == 0 || secondByKey.Count == 0) return false;
			int firstLow = 0;
			foreach (uint k in firstByKey.Keys) if (IsLowRange(k)) firstLow++;
			int secondLow = 0;
			foreach (uint k in secondByKey.Keys) if (IsLowRange(k)) secondLow++;
			double firstRatio = firstLow / (double)firstByKey.Count;
			double secondRatio = secondLow / (double)secondByKey.Count;
			return (firstRatio >= 0.7 && secondRatio <= 0.3) || (secondRatio >= 0.7 && firstRatio <= 0.3);
		}

		private static void CompareListByIndex<T>(string path, IList<T> first, IList<T> second, DifferenceCollector collector, HashSet<string> visitedPairs, Type elemType) {
			int firstCount = first?.Count ?? 0;
			int secondCount = second?.Count ?? 0;
			if (firstCount != secondCount) {
				collector.Add($"{path}: count {firstCount} != {secondCount}");
				if (collector.IsTruncated) return;
			}
			int min = Math.Min(firstCount, secondCount);
			for (int i = 0; i < min; i++) {
				CompareObject($"{path}.byIndex[{i}]", first[i], second[i], elemType, collector, visitedPairs);
				if (collector.IsTruncated) return;
			}
		}

		private static void CompareListByKey<T>(string path, IList<T> first, IList<T> second, Func<T, uint> keySelector, DifferenceCollector collector, HashSet<string> visitedPairs, Type elemType) {
			Dictionary<uint, T> firstByKey = new Dictionary<uint, T>();
			Dictionary<uint, T> secondByKey = new Dictionary<uint, T>();

			if (first != null) {
				for (int i = 0; i < first.Count; i++) {
					uint key = keySelector(first[i]);
					if (!firstByKey.ContainsKey(key)) firstByKey[key] = first[i];
				}
			}
			if (second != null) {
				for (int i = 0; i < second.Count; i++) {
					uint key = keySelector(second[i]);
					if (!secondByKey.ContainsKey(key)) secondByKey[key] = second[i];
				}
			}

			if (HasDifferentKeyNamespace(firstByKey, secondByKey)) {
				collector.Add($"{path}: key namespaces differ strongly (likely game-specific IDs), fallback compare by index");
				if (collector.IsTruncated) return;
				CompareListByIndex(path, first, second, collector, visitedPairs, elemType);
				return;
			}

			foreach (uint key in firstByKey.Keys) {
				if (!secondByKey.ContainsKey(key)) {
					collector.Add($"{path}: missing in second key 0x{key:X8}");
					if (collector.IsTruncated) return;
				}
			}
			foreach (uint key in secondByKey.Keys) {
				if (!firstByKey.ContainsKey(key)) {
					collector.Add($"{path}: missing in first key 0x{key:X8}");
					if (collector.IsTruncated) return;
				}
			}

			foreach (uint key in firstByKey.Keys) {
				if (!secondByKey.TryGetValue(key, out T secondItem)) continue;
				CompareObject($"{path}[0x{key:X8}]", firstByKey[key], secondItem, elemType, collector, visitedPairs);
				if (collector.IsTruncated) return;
			}
		}

		private class DifferenceCollector {
			public int MaxDifferences { get; }
			public int TotalDifferences { get; private set; }
			public bool IsTruncated => TotalDifferences >= MaxDifferences;
			public List<string> Differences { get; } = new List<string>();
			public DifferenceCollector(int maxDifferences) {
				MaxDifferences = Math.Max(1, maxDifferences);
			}
			public void Add(string difference) {
				TotalDifferences++;
				if (Differences.Count < MaxDifferences) {
					Differences.Add(difference);
				}
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
