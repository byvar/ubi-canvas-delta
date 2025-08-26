namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class LinkCurveComponent_Template : PatchCurveComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xCB8258D4;
	}
}

