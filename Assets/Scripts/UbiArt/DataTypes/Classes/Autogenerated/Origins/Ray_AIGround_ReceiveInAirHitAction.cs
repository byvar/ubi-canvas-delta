namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGround_ReceiveInAirHitAction : Ray_AIGroundReceiveHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7B55B9AD;
	}
}

