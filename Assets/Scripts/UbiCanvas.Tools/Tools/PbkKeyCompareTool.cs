using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Animation;
using UnityEngine;

namespace UbiCanvas.Tools {
	public class PbkKeyCompareTool : GameTool {
		public override string Name => "Key Compare";
		public override string Description => "Compare keys between two .pbk.ckd files without copying data (Origins/Legends/Adventures/Mini/Fiesta Run iOS).";

		public enum PbkGameLocation {
			RaymanLegends,
			RaymanOrigins,
			RaymanMini,
			RaymanAdventures,
			RaymanFiestaRuniOS,
		}

		public PbkGameLocation FirstGame { get; set; } = PbkGameLocation.RaymanLegends;
		public string FirstPBKPath { get; set; }

		public PbkGameLocation SecondGame { get; set; } = PbkGameLocation.RaymanMini;
		public string SecondPBKPath { get; set; }

		public string LastComparisonReport { get; private set; }

		public Task CompareAsync() {
			if (string.IsNullOrWhiteSpace(FirstPBKPath) || string.IsNullOrWhiteSpace(SecondPBKPath)) {
				throw new InvalidOperationException("FirstPBKPath and SecondPBKPath are required");
			}

			string firstFullPath = ResolveFilePath(FirstGame, FirstPBKPath);
			string secondFullPath = ResolveFilePath(SecondGame, SecondPBKPath);

			byte[] firstData = File.ReadAllBytes(firstFullPath);
			byte[] secondData = File.ReadAllBytes(secondFullPath);

			using Context firstContext = CreateContext(
				basePath: ResolveBundleDirectory(FirstGame),
				mode: GetModeForLocation(FirstGame),
				enableSerializerLog: false,
				loadAnimations: false,
				loadAllPaths: false,
				loadStrings: true);
			using Context secondContext = CreateContext(
				basePath: ResolveBundleDirectory(SecondGame),
				mode: GetModeForLocation(SecondGame),
				enableSerializerLog: false,
				loadAnimations: false,
				loadAllPaths: false,
				loadStrings: true);

			AnimPatchBank first = CSerializable.CreateFromBinaryData(firstData, typeof(AnimPatchBank), "pbk", firstContext, allowLoadingReferences: true) as AnimPatchBank;
			AnimPatchBank second = CSerializable.CreateFromBinaryData(secondData, typeof(AnimPatchBank), "pbk", secondContext, allowLoadingReferences: true) as AnimPatchBank;

			if (first == null || second == null) {
				throw new InvalidOperationException("Failed to deserialize one of the PBK files");
			}

			HashSet<ulong> firstKeys = GetKeys(first);
			HashSet<ulong> secondKeys = GetKeys(second);
			Dictionary<ulong, List<int>> firstKeyIndices = GetKeyIndices(first);
			Dictionary<ulong, List<int>> secondKeyIndices = GetKeyIndices(second);

			List<ulong> onlyInFirst = firstKeys.Except(secondKeys).OrderBy(k => k).ToList();
			List<ulong> onlyInSecond = secondKeys.Except(firstKeys).OrderBy(k => k).ToList();

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"Compared keys between {FirstGame} and {SecondGame}");
			sb.AppendLine($"{FirstGame} key count: {firstKeys.Count}");
			sb.AppendLine($"{SecondGame} key count: {secondKeys.Count}");
			sb.AppendLine($"Common keys: {firstKeys.Count - onlyInFirst.Count}");
			sb.AppendLine();

			sb.AppendLine($"Keys only in {FirstGame}: {onlyInFirst.Count}");
			foreach (ulong key in onlyInFirst) {
				sb.AppendLine($"- key {FormatKey(key)} [{FormatKeyIndices(key, firstKeyIndices)}] ({FormatKeyString(key, firstContext)}) only in {FirstGame}");
			}

			sb.AppendLine();
			sb.AppendLine($"Keys only in {SecondGame}: {onlyInSecond.Count}");
			foreach (ulong key in onlyInSecond) {
				sb.AppendLine($"- key {FormatKey(key)} [{FormatKeyIndices(key, secondKeyIndices)}] ({FormatKeyString(key, secondContext)}) only in {SecondGame}");
			}

			LastComparisonReport = sb.ToString();
			Debug.Log(LastComparisonReport);
			firstContext.SystemLogger?.LogInfo(LastComparisonReport);

			return Task.CompletedTask;
		}

		private static HashSet<ulong> GetKeys(AnimPatchBank pbk) {
			HashSet<ulong> result = new();
			if (pbk?.templateKeys?.keys == null || pbk.templates == null) return result;

			int count = pbk.templates.Count;
			for (int i = 0; i < count; i++) {
				result.Add(pbk.templateKeys.GetKeyFromValue(i));
			}
			return result;
		}

		private static Dictionary<ulong, List<int>> GetKeyIndices(AnimPatchBank pbk) {
			Dictionary<ulong, List<int>> result = new();
			if (pbk?.templateKeys?.keys == null || pbk.templates == null) return result;

			int count = pbk.templates.Count;
			for (int i = 0; i < count; i++) {
				ulong key = pbk.templateKeys.GetKeyFromValue(i);
				if (!result.TryGetValue(key, out List<int> indices)) {
					indices = new List<int>();
					result[key] = indices;
				}
				indices.Add(i);
			}
			return result;
		}

		private static string FormatKeyIndices(ulong key, Dictionary<ulong, List<int>> keyIndices) {
			if (keyIndices == null || !keyIndices.TryGetValue(key, out List<int> indices) || indices.Count == 0)
				return "index: n/a";

			if (indices.Count == 1)
				return $"index: {indices[0]}";

			return $"indices: {string.Join(", ", indices)}";
		}

		private static string FormatKey(ulong key) => $"0x{key:X} (dec {key})";

		private static string FormatKeyString(ulong key, Context context) {
			if (context?.StringCache == null) return "str: Not found";
			if (key > uint.MaxValue) return "str: Not found";

			StringID sid = new StringID((uint)key);
			if (context.StringCache.TryGetValue(sid, out string value) && !string.IsNullOrWhiteSpace(value)) {
				return $"str: {value}";
			}
			return "str: Not found";
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
	}
}
