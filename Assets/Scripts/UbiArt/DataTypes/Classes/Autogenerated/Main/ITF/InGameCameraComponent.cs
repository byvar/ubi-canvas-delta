namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class InGameCameraComponent : BaseCameraComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1215E9F0;
	}
}

