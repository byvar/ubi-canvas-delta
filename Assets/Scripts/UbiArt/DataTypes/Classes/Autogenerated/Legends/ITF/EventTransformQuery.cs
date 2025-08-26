namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventTransformQuery : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0A63C099;
	}
}

