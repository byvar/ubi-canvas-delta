namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventVirtualLinkBroadcast : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE9DBA557;
	}
}

