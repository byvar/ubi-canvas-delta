namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventQueryGetBounceMultiplier : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA9615892;
	}
}

