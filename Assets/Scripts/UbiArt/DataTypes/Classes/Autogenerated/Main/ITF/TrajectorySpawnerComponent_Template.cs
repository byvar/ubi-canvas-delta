namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class TrajectorySpawnerComponent_Template : ActorComponent_Template {
		public CListO<Path> spawneePaths;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			spawneePaths = s.SerializeObject<CListO<Path>>(spawneePaths, name: "spawneePaths");
		}
		public override uint? ClassCRC => 0x01F2EC37;
	}
}

