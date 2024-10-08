namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_MazePortalComponent : ActorComponent {
		public Vec2d wallPos;
		public Vec2d wallExtent;
		public bool debug;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				wallPos = s.SerializeObject<Vec2d>(wallPos, name: "wallPos");
				wallExtent = s.SerializeObject<Vec2d>(wallExtent, name: "wallExtent");
				debug = s.Serialize<bool>(debug, name: "debug");
			}
		}
		public override uint? ClassCRC => 0xE0B41115;
	}
}

