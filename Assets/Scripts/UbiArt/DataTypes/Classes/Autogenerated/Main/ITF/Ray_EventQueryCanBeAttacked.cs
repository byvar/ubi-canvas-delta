namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventQueryCanBeAttacked : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBAD08C8C;
	}
}

