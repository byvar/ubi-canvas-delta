namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventHitSuccessful : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x30CF29C5;
	}
}

