namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class FixedCameraComponent : BaseCameraComponent {
		public Vec3d offset;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				offset = s.SerializeObject<Vec3d>(offset, name: "offset");
			}
		}
		public override uint? ClassCRC => 0x461D3103;
	}
}

