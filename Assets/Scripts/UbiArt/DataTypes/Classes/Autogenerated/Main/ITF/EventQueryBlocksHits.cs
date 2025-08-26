namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventQueryBlocksHits : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x07F8710C;
	}
}

