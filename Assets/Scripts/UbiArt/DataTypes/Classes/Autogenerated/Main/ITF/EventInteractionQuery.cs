namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventInteractionQuery : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0C7E4060;
	}
}

