namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGroundAttackBehavior : Ray_AIGroundBaseMovementAttackBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4EB73F87;
	}
}

