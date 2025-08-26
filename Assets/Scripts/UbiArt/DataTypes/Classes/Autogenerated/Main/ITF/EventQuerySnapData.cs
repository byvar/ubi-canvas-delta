namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventQuerySnapData : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2C92A620;
	}
}

