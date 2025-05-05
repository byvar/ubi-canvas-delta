namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class RenderBezierPatchCurveComponent_Template : CSerializable {
		public BezierCurveRenderer_Template bezierRenderer;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			bezierRenderer = s.SerializeObject<BezierCurveRenderer_Template>(bezierRenderer, name: "bezierRenderer");
		}
		public override uint? ClassCRC => 0x7C8C6C33;
	}
}

