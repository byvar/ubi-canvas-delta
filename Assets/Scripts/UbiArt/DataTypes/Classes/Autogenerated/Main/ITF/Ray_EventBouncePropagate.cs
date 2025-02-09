namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RJR | GameFlags.RFR)]
	public partial class Ray_EventBouncePropagate : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x34FFDAED;
	}
}

