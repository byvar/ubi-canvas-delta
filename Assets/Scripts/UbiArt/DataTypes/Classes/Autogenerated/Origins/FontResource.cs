namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class FontResource : Resource {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFA52E6BF;
	}
}

