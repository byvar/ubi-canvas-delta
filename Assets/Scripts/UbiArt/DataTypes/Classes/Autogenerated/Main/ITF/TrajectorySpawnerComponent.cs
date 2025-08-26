namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class TrajectorySpawnerComponent : ActorComponent {
		public Placeholder spawneePaths;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				spawneePaths = s.SerializeObject<Placeholder>(spawneePaths, name: "spawneePaths");
			}
		}
		public override uint? ClassCRC => 0x629061C6;
	}
}

