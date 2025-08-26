namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class BodyPartSprite : BodyPartBase {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2FD2F7CB;
	}
}

