namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class FollowCamComponent : ActorComponent {
		public Vec3d offsetFromCam;
		public bool ignoreCamZ;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					offsetFromCam = s.SerializeObject<Vec3d>(offsetFromCam, name: "offsetFromCam");
				}
				ignoreCamZ = s.Serialize<bool>(ignoreCamZ, name: "ignoreCamZ");
			} else {
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					offsetFromCam = s.SerializeObject<Vec3d>(offsetFromCam, name: "offsetFromCam");
				}
				ignoreCamZ = s.Serialize<bool>(ignoreCamZ, name: "ignoreCamZ");
			}
		}
		public override uint? ClassCRC => 0x03300214;
	}
}

