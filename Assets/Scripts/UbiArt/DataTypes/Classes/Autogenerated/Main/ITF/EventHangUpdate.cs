namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventHangUpdate : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB5C26542;
	}
}

