namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_PlatformerCameraComponent : InGameCameraComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x36DE46F0;
	}
}

