namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventPlayerControllerIdChange : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1F7D234B;
	}
}

