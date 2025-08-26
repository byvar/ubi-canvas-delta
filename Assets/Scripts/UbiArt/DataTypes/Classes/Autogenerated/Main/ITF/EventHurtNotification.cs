namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventHurtNotification : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2D2FFE14;
	}
}

