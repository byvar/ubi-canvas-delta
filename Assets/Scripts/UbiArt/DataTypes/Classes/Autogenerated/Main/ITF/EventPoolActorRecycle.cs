namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventPoolActorRecycle : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6EDBA6D9;
	}
}

