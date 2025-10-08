namespace UbiArt.ITF {
	[Games(GameFlags.RM)]
	public partial class RLC_WorldButton : CSerializable {
		public Generic<Event> eventSentWhenSpawned;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			eventSentWhenSpawned = s.SerializeObject<Generic<Event>>(eventSentWhenSpawned, name: "eventSentWhenSpawned");
		}
		public override uint? ClassCRC => 0x90CDAC93;
	}
}

