namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventQueryFaction : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xDB439242;
	}
}

