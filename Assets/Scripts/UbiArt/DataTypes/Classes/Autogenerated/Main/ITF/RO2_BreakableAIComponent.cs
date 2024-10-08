namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_BreakableAIComponent : RO2_AIComponent {
		public uint currentDestructionStage;
		public uint targetDestructionStage;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				currentDestructionStage = s.Serialize<uint>(currentDestructionStage, name: "currentDestructionStage");
				targetDestructionStage = s.Serialize<uint>(targetDestructionStage, name: "targetDestructionStage");
			}
		}
		public override uint? ClassCRC => 0x38CD061E;
	}
}

