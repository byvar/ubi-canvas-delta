namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_ArmComponent : ActorComponent {
		public uint activationState = 0xFFFFFFFF;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				activationState = s.Serialize<uint>(activationState, name: "activationState");
			}
		}
		public override uint? ClassCRC => 0x50767C54;
	}
}

