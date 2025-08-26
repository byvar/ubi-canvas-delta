namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BossCameraComponent : BaseCameraComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC525EC6C;
	}
}

