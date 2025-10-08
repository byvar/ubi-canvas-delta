namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_GameStatsComponent_Template : ActorComponent_Template {
		public Path scoreTextActor;
		public float fontHeight;
		public Color textColor;
		public CArrayO<Vec2d> scoreOffsets2D;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			scoreTextActor = s.SerializeObject<Path>(scoreTextActor, name: "scoreTextActor");
			fontHeight = s.Serialize<float>(fontHeight, name: "fontHeight");
			textColor = s.SerializeObject<Color>(textColor, name: "textColor");
			scoreOffsets2D = s.SerializeObject<CArrayO<Vec2d>>(scoreOffsets2D, name: "scoreOffsets2D");
		}
		public override uint? ClassCRC => 0x921DF538;
	}
}

