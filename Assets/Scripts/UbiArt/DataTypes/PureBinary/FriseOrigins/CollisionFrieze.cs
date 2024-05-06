namespace UbiArt.FriseOrigins {
	// See: ITF::CollisionFrieze::serialize
	public class CollisionFrieze : CSerializable {
		public float offset;
		public bool build;
		public bool cornerInCur;
		public bool cornerOutCur;
		public Vec2d extremity;
		public Vec2d extremity2;
		public Vec2d cornerIn;
		public Vec2d cornerOut;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			offset = s.Serialize<float>(offset, name: "offset");
			build = s.Serialize<bool>(build, name: "build");
			cornerInCur = s.Serialize<bool>(cornerInCur, name: "cornerInCur");
			cornerOutCur = s.Serialize<bool>(cornerOutCur, name: "cornerOutCur");
			extremity = s.SerializeObject<Vec2d>(extremity, name: "extremity");
			extremity2 = s.SerializeObject<Vec2d>(extremity2, name: "extremity2");
			cornerIn = s.SerializeObject<Vec2d>(cornerIn, name: "cornerIn");
			cornerOut = s.SerializeObject<Vec2d>(cornerOut, name: "cornerOut");
		}
	}
}
