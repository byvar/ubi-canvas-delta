namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventSetTarget : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF26F313B;
	}
}

