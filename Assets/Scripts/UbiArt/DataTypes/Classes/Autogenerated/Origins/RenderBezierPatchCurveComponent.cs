namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class RenderBezierPatchCurveComponent : GraphicComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA3EB1A4B;
	}
}

