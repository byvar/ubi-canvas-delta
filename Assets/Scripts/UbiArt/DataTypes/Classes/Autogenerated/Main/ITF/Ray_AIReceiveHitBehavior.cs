namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIReceiveHitBehavior : AIReceiveHitBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x87C6018B;
	}
}

