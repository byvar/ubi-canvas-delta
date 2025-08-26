namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class EventWallJump : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x93A301E2;
	}
}

