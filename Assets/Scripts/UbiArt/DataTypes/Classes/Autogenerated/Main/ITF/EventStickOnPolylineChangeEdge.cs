namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventStickOnPolylineChangeEdge : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFBB34E6F;
	}
}

