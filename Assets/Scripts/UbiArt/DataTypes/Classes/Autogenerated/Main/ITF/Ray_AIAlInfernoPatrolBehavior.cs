namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIAlInfernoPatrolBehavior : Ray_AIGroundBaseMovementBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4DDB3C3D;
	}
}

