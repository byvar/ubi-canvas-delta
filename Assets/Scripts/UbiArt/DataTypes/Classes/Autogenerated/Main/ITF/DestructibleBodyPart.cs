namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class DestructibleBodyPart : BodyPart {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x04D77336;
	}
}

