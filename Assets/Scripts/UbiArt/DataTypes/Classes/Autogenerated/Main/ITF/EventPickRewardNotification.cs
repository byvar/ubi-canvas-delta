namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventPickRewardNotification : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6A40E93D;
	}
}

