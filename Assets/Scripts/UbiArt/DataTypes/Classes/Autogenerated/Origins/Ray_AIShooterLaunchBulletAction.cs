namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIShooterLaunchBulletAction : Ray_AIPerformHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x113AE282;
	}
}

