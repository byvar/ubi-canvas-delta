namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIPerformHitPolylineJumpPunchAction : Ray_AIPerformHitPolylinePunchAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x85EA065F;
	}
}

