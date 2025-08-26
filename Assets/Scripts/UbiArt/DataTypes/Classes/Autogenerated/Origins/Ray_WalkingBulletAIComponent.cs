namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WalkingBulletAIComponent : Ray_BulletAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2294243D;
	}
}

