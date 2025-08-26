namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AILivingStoneRoamBehavior : Ray_AIGroundBaseMovementBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x563242EF;
	}
}

