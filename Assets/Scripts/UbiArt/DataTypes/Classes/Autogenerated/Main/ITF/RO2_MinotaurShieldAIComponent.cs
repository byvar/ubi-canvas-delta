namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_MinotaurShieldAIComponent : RO2_AIComponent {
		public bool standOnce = true;
		public bool useDestructionDelay = true;
		public bool dead;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				standOnce = s.Serialize<bool>(standOnce, name: "standOnce", options: CSerializerObject.Options.BoolAsByte);
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					dead = s.Serialize<bool>(dead, name: "dead");
				}
			} else {
				standOnce = s.Serialize<bool>(standOnce, name: "standOnce");
				useDestructionDelay = s.Serialize<bool>(useDestructionDelay, name: "useDestructionDelay");
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					dead = s.Serialize<bool>(dead, name: "dead");
				}
			}
		}
		public override uint? ClassCRC => 0x126B81D8;
	}
}

