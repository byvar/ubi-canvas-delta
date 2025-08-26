namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIHunterLaunchBulletAction : Ray_AIPerformHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xECB3E21A;
	}
}

