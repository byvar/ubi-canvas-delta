namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventQueryPhysShape : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7BFEA4ED;
	}
}

