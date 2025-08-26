namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventOrangeContact : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF0D170EE;
	}
}

