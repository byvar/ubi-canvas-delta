namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventScaleChanged : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD862CFC6;
	}
}

