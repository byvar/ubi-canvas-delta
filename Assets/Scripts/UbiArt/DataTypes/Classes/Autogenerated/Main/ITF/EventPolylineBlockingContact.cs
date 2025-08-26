namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventPolylineBlockingContact : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x76C6D843;
	}
}

