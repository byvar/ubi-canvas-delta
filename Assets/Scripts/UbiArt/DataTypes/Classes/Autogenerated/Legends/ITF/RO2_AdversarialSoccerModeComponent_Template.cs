namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_AdversarialSoccerModeComponent_Template : RO2_AdversarialModeComponent_Template {
		public Path ballPath;
		public uint goalLumCount = 1;
		public uint goalOnFireLumCount = 2;
		public float goalRespawnDelay = 3;
		public float ballDestroyDelay = 0.5f;
		public float maxBallCount = 3;
		public float multiBallCooldown = 1;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			ballPath = s.SerializeObject<Path>(ballPath, name: "ballPath");
			goalLumCount = s.Serialize<uint>(goalLumCount, name: "goalLumCount");
			goalOnFireLumCount = s.Serialize<uint>(goalOnFireLumCount, name: "goalOnFireLumCount");
			goalRespawnDelay = s.Serialize<float>(goalRespawnDelay, name: "goalRespawnDelay");
			ballDestroyDelay = s.Serialize<float>(ballDestroyDelay, name: "ballDestroyDelay");
			maxBallCount = s.Serialize<float>(maxBallCount, name: "maxBallCount");
			multiBallCooldown = s.Serialize<float>(multiBallCooldown, name: "multiBallCooldown");
		}
		public override uint? ClassCRC => 0x345EABDC;
	}
}

