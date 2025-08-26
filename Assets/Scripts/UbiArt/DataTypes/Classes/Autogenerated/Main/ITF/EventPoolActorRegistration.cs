namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventPoolActorRegistration : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB5039023;
	}
}

