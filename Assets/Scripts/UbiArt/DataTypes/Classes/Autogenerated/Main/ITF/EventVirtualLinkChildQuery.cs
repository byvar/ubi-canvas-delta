namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventVirtualLinkChildQuery : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3812D747;
	}
}

