namespace UbiArt.SceneConfig {
	public class SceneConfigManager : CSerializable {
		public uint version;
		public uint dataversion;
		public CMapGeneric<StringID, ITF.SceneConfig> sgsMap;
		public SgsKey sgsMapAdv;

		protected override void SerializeImpl(CSerializerObject s) {
			if(s.Settings.Game == Game.RA || s.Settings.Game == Game.RM) {
				version = s.SerializeGenericPureBinary<uint>(version, name: "unk");
				sgsMapAdv = s.SerializeObject<SgsKey>(sgsMapAdv, name: "sgsMap");
			} else {
				version = s.Serialize<uint>(version, name: "unk");
				dataversion = s.Serialize<uint>(dataversion, name: "dataversion");
				sgsMap = s.SerializeObject<CMapGeneric<StringID, ITF.SceneConfig>>(sgsMap, name: "sgsMap");
			}
		}

		public class SgsKey : CSerializable {
			public CMap<StringID, Generic<ITF.SceneConfig>> Keys;

			protected override void SerializeImpl(CSerializerObject s) {
				Keys = s.SerializeObject<CMap<StringID, Generic<ITF.SceneConfig>>>(Keys, name: "Keys");
			}

			public override string ClassName => "SceneConfigManager::SgsKey";

			//public override uint? ClassCRC => new StringID("SceneConfigManager::SgsKey");
		}
	}
}
