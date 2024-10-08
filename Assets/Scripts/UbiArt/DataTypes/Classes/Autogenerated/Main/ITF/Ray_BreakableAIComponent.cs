namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BreakableAIComponent : Ray_AIComponent {
		public uint currentDestructionStage;
		public uint targetDestructionStage;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				currentDestructionStage = s.Serialize<uint>(currentDestructionStage, name: "currentDestructionStage");
				targetDestructionStage = s.Serialize<uint>(targetDestructionStage, name: "targetDestructionStage");
			}
		}
		public override uint? ClassCRC => 0x6E1967DA;
	}
}

