namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CheckpointComponent : CheckpointComponent {
		public Placeholder SpawnPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				SpawnPos = s.SerializeObject<Placeholder>(SpawnPos, name: "SpawnPos");
			}
		}
		public override uint? ClassCRC => 0x413DDAA2;
	}
}

