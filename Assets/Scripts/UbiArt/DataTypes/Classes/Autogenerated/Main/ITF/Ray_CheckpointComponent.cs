namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CheckpointComponent : CheckpointComponent {
		public CArrayO<Vec2d> SpawnPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				SpawnPos = s.SerializeObject<CArrayO<Vec2d>>(SpawnPos, name: "SpawnPos");
			}
		}
		public override uint? ClassCRC => 0x413DDAA2;
	}
}

