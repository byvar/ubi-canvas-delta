namespace UbiArt.Animation {
	// See: ITF::PatchData::serialize
	public class PatchData : CSerializable {
		public ushort uvsIndex;
		public byte alphaBegin; // Might be inverted! it's a ushort alphaBeginEnd
		public byte alphaEnd;
		public Vec2d[] points = new Vec2d[4];

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			uvsIndex = s.Serialize<ushort>(uvsIndex, name: "uvsIndex");
			alphaBegin = s.Serialize<byte>(alphaBegin, name: "alphaBegin");
			alphaEnd = s.Serialize<byte>(alphaEnd, name: "alphaEnd");
			for (int i = 0; i < points.Length; i++) {
				points[i] = s.SerializeObject<Vec2d>(points[i], name: $"points[{i}]");
			}
		}
	}
}
