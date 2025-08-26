namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_SeekingBulletAIComponent : Ray_BulletAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3B5F23C1;
	}
}

