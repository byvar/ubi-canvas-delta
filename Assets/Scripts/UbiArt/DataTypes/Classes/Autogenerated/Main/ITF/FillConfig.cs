namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class FillConfig : CSerializable {
		public float smoothFactor = float.MaxValue;
		public float offset = 0.5f;
		public Angle angle;
		public Vec2d scale = new Vec2d(2, 2);
		public StringID tex;
		public float zExtrude;
		public bool isSmooth;
		public int texIndex = -1;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			smoothFactor = s.Serialize<float>(smoothFactor, name: "smoothFactor");
			offset = s.Serialize<float>(offset, name: "offset");
			angle = s.SerializeObject<Angle>(angle, name: "angle");
			scale = s.SerializeObject<Vec2d>(scale, name: "scale");
			tex = s.SerializeObject<StringID>(tex, name: "tex");
			zExtrude = s.Serialize<float>(zExtrude, name: "zExtrude");
			if (s.HasFlags(SerializeFlags.DataBin)) {
				if (s.Settings.Game == Game.COL) {
					isSmooth = s.Serialize<bool>(isSmooth, name: "isSmooth", options: CSerializerObject.Options.BoolAsByte);
				} else {
					isSmooth = s.Serialize<bool>(isSmooth, name: "isSmooth");
				}
				texIndex = s.Serialize<int>(texIndex, name: "texIndex");
			}
		}
	}
}

