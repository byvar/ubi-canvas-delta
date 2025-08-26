namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGround_ReceiveHitLaunchBulletAction : Ray_AIGround_ReceiveNormalHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE7D8C73A;
	}
}

