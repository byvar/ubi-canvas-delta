namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_PickupManager_Template : TemplateObj {
		public Path scoreLumPath;
		public Path heartRainPath;
		public Path heartNfcPath;
		public float heartRainCooldown = 10f;
		public Path DRCItemHeartPath;
		public float DRCItemHeartWaitDurationMin = 10;
		public float DRCItemHeartWaitDurationMax = 60;
		public uint DRCItemHeartMinDeathCount = 4;
		public uint DRCItemHeartMaxDeathCount = 10;
		public uint DRCItemHeartMaxByCheckpoint = 3;
		public Path prisonerCounterPath;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				prisonerCounterPath = s.SerializeObject<Path>(prisonerCounterPath, name: "prisonerCounterPath");
				scoreLumPath = s.SerializeObject<Path>(scoreLumPath, name: "scoreLumPath");
				heartRainPath = s.SerializeObject<Path>(heartRainPath, name: "heartRainPath");
				heartNfcPath = s.SerializeObject<Path>(heartNfcPath, name: "heartNfcPath");
				heartRainCooldown = s.Serialize<float>(heartRainCooldown, name: "heartRainCooldown");
				DRCItemHeartPath = s.SerializeObject<Path>(DRCItemHeartPath, name: "DRCItemHeartPath");
				DRCItemHeartWaitDurationMin = s.Serialize<float>(DRCItemHeartWaitDurationMin, name: "DRCItemHeartWaitDurationMin");
				DRCItemHeartWaitDurationMax = s.Serialize<float>(DRCItemHeartWaitDurationMax, name: "DRCItemHeartWaitDurationMax");
				DRCItemHeartMinDeathCount = s.Serialize<uint>(DRCItemHeartMinDeathCount, name: "DRCItemHeartMinDeathCount");
				DRCItemHeartMaxDeathCount = s.Serialize<uint>(DRCItemHeartMaxDeathCount, name: "DRCItemHeartMaxDeathCount");
				DRCItemHeartMaxByCheckpoint = s.Serialize<uint>(DRCItemHeartMaxByCheckpoint, name: "DRCItemHeartMaxByCheckpoint");
			} else {
				scoreLumPath = s.SerializeObject<Path>(scoreLumPath, name: "scoreLumPath");
				heartRainPath = s.SerializeObject<Path>(heartRainPath, name: "heartRainPath");
				heartNfcPath = s.SerializeObject<Path>(heartNfcPath, name: "heartNfcPath");
				heartRainCooldown = s.Serialize<float>(heartRainCooldown, name: "heartRainCooldown");
				DRCItemHeartPath = s.SerializeObject<Path>(DRCItemHeartPath, name: "DRCItemHeartPath");
				DRCItemHeartWaitDurationMin = s.Serialize<float>(DRCItemHeartWaitDurationMin, name: "DRCItemHeartWaitDurationMin");
				DRCItemHeartWaitDurationMax = s.Serialize<float>(DRCItemHeartWaitDurationMax, name: "DRCItemHeartWaitDurationMax");
				DRCItemHeartMinDeathCount = s.Serialize<uint>(DRCItemHeartMinDeathCount, name: "DRCItemHeartMinDeathCount");
				DRCItemHeartMaxDeathCount = s.Serialize<uint>(DRCItemHeartMaxDeathCount, name: "DRCItemHeartMaxDeathCount");
				DRCItemHeartMaxByCheckpoint = s.Serialize<uint>(DRCItemHeartMaxByCheckpoint, name: "DRCItemHeartMaxByCheckpoint");
			}
		}
		public override uint? ClassCRC => 0xD2C166E6;
	}
}

