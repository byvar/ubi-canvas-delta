namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventForceStickOnEdge : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEB4015EB;
	}
}

