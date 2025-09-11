namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_GhostManager_Template : TemplateObj {
		public Path ghostPath;
		public CListO<RO2_GhostColor> config;

		public Color ghostFactorColorDefault = Color.White;
		public Color ghostFactorColorMin = Color.White;
		public Color ghostFactorColorMax = Color.White;
		public Color ghostFogColorDefault;
		public Color ghostFogColorMin;
		public Color ghostFogColorMax;
		public float ghostDistColorMin;
		public float ghostDistColorMax;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			ghostPath = s.SerializeObject<Path>(ghostPath, name: "ghostPath");
			if (s.Settings.Platform == GamePlatform.Vita) {
				ghostFactorColorDefault = s.SerializeObject<Color>(ghostFactorColorDefault, name: "ghostFactorColorDefault");
				ghostFactorColorMin = s.SerializeObject<Color>(ghostFactorColorMin, name: "ghostFactorColorMin");
				ghostFactorColorMax = s.SerializeObject<Color>(ghostFactorColorMax, name: "ghostFactorColorMax");
				ghostFogColorDefault = s.SerializeObject<Color>(ghostFogColorDefault, name: "ghostFogColorDefault");
				ghostFogColorMin = s.SerializeObject<Color>(ghostFogColorMin, name: "ghostFogColorMin");
				ghostFogColorMax = s.SerializeObject<Color>(ghostFogColorMax, name: "ghostFogColorMax");
				ghostDistColorMin = s.Serialize<float>(ghostDistColorMin, name: "ghostDistColorMin");
				ghostDistColorMax = s.Serialize<float>(ghostDistColorMax, name: "ghostDistColorMax");
			} else {
				config = s.SerializeObject<CListO<RO2_GhostColor>>(config, name: "config");
			}
		}
		public override uint? ClassCRC => 0x2F7CAAAD;
	}
}

