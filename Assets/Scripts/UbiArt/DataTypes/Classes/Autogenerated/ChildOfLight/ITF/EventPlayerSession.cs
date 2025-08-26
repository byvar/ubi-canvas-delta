namespace UbiArt.ITF {
	[Games(GameFlags.COL | GameFlags.RM)]
	public partial class EventPlayerSession : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x72DE1623;
	}
}

