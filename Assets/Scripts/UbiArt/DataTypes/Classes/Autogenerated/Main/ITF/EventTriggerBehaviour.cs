namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventTriggerBehaviour : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x84A15F7A;
	}
}

