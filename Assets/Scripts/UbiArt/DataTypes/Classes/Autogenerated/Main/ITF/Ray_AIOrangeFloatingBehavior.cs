namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIOrangeFloatingBehavior : Ray_AIWaterFloatingBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3F3BF7A2;
	}
}

