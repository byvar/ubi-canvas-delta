namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventDead : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x25D51138;
	}
}

