namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.VH | GameFlags.RA)]
	public partial class AIReceiveHitBehavior : AIBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xDABF8A41;
	}
}

