namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class LinkCurveComponent : PatchCurveComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBA352417;
	}
}

