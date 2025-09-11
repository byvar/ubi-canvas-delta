#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace UbiArt
{
	public class Settings
	{
		#region Public Properties

		// TODO: The default values are a bit all over the place. Perhaps pick one to use as default? Or require parameters from constructor?
		public Mode Mode { get; set; } = Mode.RaymanLegendsPC;
		public EngineVersion EngineVersion { get; set; } = EngineVersion.RO;
		public Game Game { get; set; } = Game.None;
		public GamePlatform Platform { get; set; } = GamePlatform.None;
		public SerializerType SerializerType { get; set; } = SerializerType.Binary;
		public Endian Endian { get; set; } = Endian.Little;
		public VersionFlags VersionFlags { get; set; } = VersionFlags.None;
		public bool UsesSerializeFlags { get; set; } = true;
		public bool Cooked { get; set; } = true;
		public bool PastaStructure { get; set; } = false;
		public string[]? Bundles { get; set; }
		public uint IpkVersion { get; set; }
		public uint EngineSignature { get; set; }
		public IMemoryData? MemoryData { get; set; }
		public Encoding DefaultEncoding { get; set; } = Encoding.GetEncoding(1252);
		public bool LoadInPlace { get; set; }

		public bool IsLittleEndian => Endian == Endian.Little;

		public bool IsDemo { get; set; } = false; // TODO: Replace with version tree?
		public bool HasInvasionsPatch { get; set; } = true; // TODO: Replace with version tree?

		public string PlatformString => Platform switch
		{
			GamePlatform.PC => "PC",
			GamePlatform.Android => "android",
			GamePlatform.iOS => "ios",
			GamePlatform.AppleTV => "fruity",
			GamePlatform.MacOS => "macos",
			GamePlatform.Vita => "VITA",
			GamePlatform.PC32 => "PC32",
			_ => throw new InvalidOperationException($"Can not get a platform string for the platform {Platform}")
		};

		public string ITFDirectory
		{
			get
			{
				if (!Cooked || PastaStructure)
					return String.Empty;
				else if (EngineVersion > EngineVersion.RO)
					return $"cache/itf_cooked/{PlatformString.ToLowerInvariant()}/";
				else
					return $"itf_cooked/{PlatformString.ToLowerInvariant()}/";
			}
		}

		public string ITFCacheDirectory
		{
			get
			{
				if (!Cooked || PastaStructure)
					return String.Empty;
				else if (EngineVersion > EngineVersion.RO)
					return $"cache/itf_cache/{PlatformString.ToLowerInvariant()}/";
				else
					return $"itf_cache/{PlatformString.ToLowerInvariant()}/";
			}
		}

		#endregion

		#region Defined Settings

		public static Settings RO_PC = new()
		{
			EngineVersion = EngineVersion.RO,
			Game = Game.RO,
			Platform = GamePlatform.PC,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new[] { "bundle" }
		};
		public static Settings RO_Vita = new() {
			EngineVersion = EngineVersion.RO,
			Game = Game.RO,
			Platform = GamePlatform.Vita,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new[] { "bundle" }
		};

		public static Settings RJR_Android = new() {
			EngineVersion = EngineVersion.RO,
			Game = Game.RJR,
			Platform = GamePlatform.Android,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			PastaStructure = true,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new string[0]
		};
		public static Settings RJR_iOS = new() {
			EngineVersion = EngineVersion.RO,
			Game = Game.RJR,
			Platform = GamePlatform.iOS,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			PastaStructure = true,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new string[0]
		};

		public static Settings RFR_Android = new() {
			EngineVersion = EngineVersion.RO,
			Game = Game.RFR,
			Platform = GamePlatform.Android,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			PastaStructure = true,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new string[0]
		};

		public static Settings RFR_iOS = new() {
			EngineVersion = EngineVersion.RO,
			Game = Game.RFR,
			Platform = GamePlatform.iOS,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Origins,
			PastaStructure = true,
			UsesSerializeFlags = false,
			IpkVersion = 3,
			EngineSignature = 0x345429C7,
			Bundles = new string[0]
		};

		public static Settings RL_PC = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.RL,
			Platform = GamePlatform.PC,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Legends,
			IpkVersion = 5,
			EngineSignature = 0x4BFC7C03,
			Bundles = new[] { "persistentLoading", "Bundle" },
			MemoryData = new MemoryData_RaymanLegendsPC(),
			LoadInPlace = true
		};

		public static Settings RA_iOS = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.RA,
			Platform = GamePlatform.iOS,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Adventures,
			SerializerType = SerializerType.TagBinary,
			IpkVersion = 8,
			EngineSignature = 0x2FB967E7,
			Bundles = new[]
			{
				"bundle",
				"bundlemain",
				"bundleonboardingadv1",
				"bundleonboardingadv2",
				"bundleonboardingadv3",
				"bundlepersonal",
				"bundlemedieval",
				"bundleshaolin",
				"bundletoadstory",
				"bundleunderwater",
				"bundledesert",
				"bundlegreece",
				"bundlelod",
				"fulllogic",
				"fulllogicmain",
				"fulllogiconboardingadv1",
				"fulllogiconboardingadv2",
				"fulllogiconboardingadv3",
				"fulllogicpersonal",
				"fulllogicmedieval",
				"fulllogicshaolin",
				"fulllogictoadstory",
				"fulllogicunderwater",
				"fulllogicdesert",
				"fulllogicgreece",
				"fulllogiclod"

			}
		};

		public static Settings RA_ATV = new() {
			EngineVersion = EngineVersion.RL,
			Game = Game.RA,
			Platform = GamePlatform.AppleTV,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Adventures,
			SerializerType = SerializerType.TagBinary,
			IpkVersion = 8,
			EngineSignature = 0x2FB967E7,
			Bundles = new[]
			{
				"bundle",
				"bundlemain",
				"bundleonboardingadv1",
				"bundleonboardingadv2",
				"bundleonboardingadv3",
				"bundlepersonal",
				"bundlemedieval",
				"bundleshaolin",
				"bundletoadstory",
				"bundleunderwater",
				"bundledesert",
				"bundlegreece",
				"bundlelod",
				"fulllogic",
				"fulllogicmain",
				"fulllogiconboardingadv1",
				"fulllogiconboardingadv2",
				"fulllogiconboardingadv3",
				"fulllogicpersonal",
				"fulllogicmedieval",
				"fulllogicshaolin",
				"fulllogictoadstory",
				"fulllogicunderwater",
				"fulllogicdesert",
				"fulllogicgreece",
				"fulllogiclod"

			}
		};

		public static Settings RA_Android = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.RA,
			Platform = GamePlatform.Android,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Adventures,
			SerializerType = SerializerType.TagBinary,
			IpkVersion = 8,
			EngineSignature = 0x2FB967E7,
			Bundles = new[] 
			{
				"bundle",
				"bundlemain",
				"bundleonboardingadv1",
				"bundleonboardingadv2",
				"bundleonboardingadv3",
				"bundlepersonal",
				"bundlemedieval",
				"bundleshaolin",
				"bundletoadstory",
				"bundleunderwater",
				"bundledesert",
				"bundlegreece",
				"bundlelod",
				"fulllogic",
				"fulllogicmain",
				"fulllogiconboardingadv1",
				"fulllogiconboardingadv2",
				"fulllogiconboardingadv3",
				"fulllogicpersonal",
				"fulllogicmedieval",
				"fulllogicshaolin",
				"fulllogictoadstory",
				"fulllogicunderwater",
				"fulllogicdesert",
				"fulllogicgreece",
				"fulllogiclod"

			}
		};

		public static Settings RM_MacOS = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.RM,
			Platform = GamePlatform.MacOS,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Adventures,
			SerializerType = SerializerType.TagBinary,
			IpkVersion = 8,
			EngineSignature = 0x2FB967E7,
			Bundles = new[] { "bundle" }
		};

		public static Settings RL_Vita = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.RL,
			Platform = GamePlatform.Vita,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Legends,
			Bundles = new[] { "Bundle" },
			LoadInPlace = true,
			HasInvasionsPatch = false,
			MemoryData = new MemoryData_RaymanLegendsVITA()
		};

		public static Settings RL_VitaPatched = new() {
			EngineVersion = EngineVersion.RL,
			Game = Game.RL,
			Platform = GamePlatform.Vita,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Legends,
			Bundles = new[] { "Bundle" },
			LoadInPlace = true,
			MemoryData = new MemoryData_RaymanLegendsVITA_Patched()
		};

		public static Settings COL_PC32 = new()
		{
			EngineVersion = EngineVersion.RL,
			Game = Game.COL,
			Platform = GamePlatform.PC32,
			Endian = Endian.Big,
			VersionFlags = VersionFlags.Legends,
			IpkVersion = 5,
			EngineSignature = 0x4BFC7C03,
			LoadInPlace = true,
			MemoryData = new MemoryData_ChildOfLightPC()
		};

		#endregion

		#region Public Methods

		public static Settings? FromMode(Mode mode)
		{
			Settings? settings = mode switch
			{
				Mode.RaymanOriginsPC => RO_PC,
				Mode.RaymanOriginsPSVita => RO_Vita,
				Mode.RaymanJungleRunAndroid => RJR_Android,
				Mode.RaymanJungleRuniOS => RJR_iOS,
				Mode.RaymanFiestaRunAndroid => RFR_Android,
				Mode.RaymanFiestaRuniOS => RFR_iOS,
				Mode.RaymanLegendsPC => RL_PC,
				Mode.RaymanLegendsPSVita => RL_Vita,
				Mode.RaymanLegendsPSVitaPatched => RL_VitaPatched,
				Mode.RaymanAdventuresAndroid => RA_Android,
				Mode.RaymanAdventuresiOS => RA_iOS,
				Mode.RaymanAdventuresATV => RA_ATV,
				Mode.RaymanMiniMacOS => RM_MacOS,
				Mode.ChildOfLightPC => COL_PC32,
				_ => null,
			};

			if (settings != null)
				settings.Mode = mode;

			return settings;
		}

		#endregion
	}
}