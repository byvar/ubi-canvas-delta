namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventGetPolylineConstrainedPosition : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBE179132;
	}
}

