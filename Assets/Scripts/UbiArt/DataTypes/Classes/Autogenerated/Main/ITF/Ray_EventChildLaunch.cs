namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventChildLaunch : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA1CBFD93;
	}
}

