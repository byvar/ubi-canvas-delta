namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_AIGroundBaseMovementAttackBehavior_Template : Ray_AIGroundBaseMovementBehavior_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE26EB2B3;
	}
}

