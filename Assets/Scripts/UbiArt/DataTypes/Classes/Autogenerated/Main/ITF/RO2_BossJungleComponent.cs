namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_BossJungleComponent : ActorComponent {
		public float SlowDownDistValidation;
		public float BaseTimeToReachMaxSpeed;
		public uint CheckPointStartIdx;
		public uint PhaseIdx;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					SlowDownDistValidation = s.Serialize<float>(SlowDownDistValidation, name: "SlowDownDistValidation");
					BaseTimeToReachMaxSpeed = s.Serialize<float>(BaseTimeToReachMaxSpeed, name: "BaseTimeToReachMaxSpeed");
					CheckPointStartIdx = s.Serialize<uint>(CheckPointStartIdx, name: "CheckPointStartIdx");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					PhaseIdx = s.Serialize<uint>(PhaseIdx, name: "PhaseIdx");
				}
			} else {
				SlowDownDistValidation = s.Serialize<float>(SlowDownDistValidation, name: "SlowDownDistValidation");
				BaseTimeToReachMaxSpeed = s.Serialize<float>(BaseTimeToReachMaxSpeed, name: "BaseTimeToReachMaxSpeed");
				CheckPointStartIdx = s.Serialize<uint>(CheckPointStartIdx, name: "CheckPointStartIdx");
			}
		}
		public override uint? ClassCRC => 0x7082AA30;
	}
}

