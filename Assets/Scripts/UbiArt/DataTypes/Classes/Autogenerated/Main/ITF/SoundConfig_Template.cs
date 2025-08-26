namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class SoundConfig_Template : TemplateObj {
		public CListO<WwiseItem> WwiseLookUpTable;
		public CListO<PathRef> WwiseBankList;
		public CListO<WwiseEngineEvent> WwiseEngineEventList;
		public CListO<EventSetWwiseAuxBusEffect> WwiseDefaultAuxEffectList;
		public float microZoffset;
		public CListO<StringID> WwiseStateToRestoreAfterHotReload;
		public CListO<BusDef> busses;
		public CListO<LimiterDef> limiters;
		public Placeholder busMixBank;
		public float pauseFadeIn;
		public float pauseFadeOut;
		public BusMix headphoneBusMix;
		public CListO<PlayerNumberBusMix> playerNumberBusMix;
		public float limiterStopFade;
		public float engineStopFade;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO) {
				busses = s.SerializeObject<CListO<BusDef>>(busses, name: "busses");
				limiters = s.SerializeObject<CListO<LimiterDef>>(limiters, name: "limiters");
				busMixBank = s.SerializeObject<Placeholder>(busMixBank, name: "busMixBank");
			} else if (s.Settings.Game == Game.RL) {
				busses = s.SerializeObject<CListO<BusDef>>(busses, name: "busses");
				limiters = s.SerializeObject<CListO<LimiterDef>>(limiters, name: "limiters");
				pauseFadeIn = s.Serialize<float>(pauseFadeIn, name: "pauseFadeIn");
				pauseFadeOut = s.Serialize<float>(pauseFadeOut, name: "pauseFadeOut");
				headphoneBusMix = s.SerializeObject<BusMix>(headphoneBusMix, name: "headphoneBusMix");
				playerNumberBusMix = s.SerializeObject<CListO<PlayerNumberBusMix>>(playerNumberBusMix, name: "playerNumberBusMix");
				limiterStopFade = s.Serialize<float>(limiterStopFade, name: "limiterStopFade");
				engineStopFade = s.Serialize<float>(engineStopFade, name: "engineStopFade");
			} else if (s.Settings.Game == Game.COL) {
				busses = s.SerializeObject<CListO<BusDef>>(busses, name: "busses");
				pauseFadeIn = s.Serialize<float>(pauseFadeIn, name: "pauseFadeIn");
				pauseFadeOut = s.Serialize<float>(pauseFadeOut, name: "pauseFadeOut");
				headphoneBusMix = s.SerializeObject<BusMix>(headphoneBusMix, name: "headphoneBusMix");
				WwiseBankList = s.SerializeObject<CListO<PathRef>>(WwiseBankList, name: "WwiseBankList");
				microZoffset = s.Serialize<float>(microZoffset, name: "microZoffset");
			} else {
				WwiseLookUpTable = s.SerializeObject<CListO<WwiseItem>>(WwiseLookUpTable, name: "WwiseLookUpTable");
				WwiseBankList = s.SerializeObject<CListO<PathRef>>(WwiseBankList, name: "WwiseBankList");
				WwiseEngineEventList = s.SerializeObject<CListO<WwiseEngineEvent>>(WwiseEngineEventList, name: "WwiseEngineEventList");
				WwiseDefaultAuxEffectList = s.SerializeObject<CListO<EventSetWwiseAuxBusEffect>>(WwiseDefaultAuxEffectList, name: "WwiseDefaultAuxEffectList");
				microZoffset = s.Serialize<float>(microZoffset, name: "microZoffset");
				WwiseStateToRestoreAfterHotReload = s.SerializeObject<CListO<StringID>>(WwiseStateToRestoreAfterHotReload, name: "WwiseStateToRestoreAfterHotReload");
			}
		}
		public override uint? ClassCRC => 0x9BB5D070;
	}
}

