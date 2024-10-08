namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_SimpleAIComponent : RO2_AIComponent {
		public bool isDead;
		public bool isStuck;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				isDead = s.Serialize<bool>(isDead, name: "isDead");
				isStuck = s.Serialize<bool>(isStuck, name: "isStuck");
			}
		}
		public override uint? ClassCRC => 0x8A326704;
	}
}

