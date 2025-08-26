namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventGetActorPlugInterface : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x145D7D1F;
	}
}

