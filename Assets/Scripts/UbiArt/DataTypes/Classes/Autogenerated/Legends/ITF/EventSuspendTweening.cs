namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventSuspendTweening : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6419F2CB;
	}
}

