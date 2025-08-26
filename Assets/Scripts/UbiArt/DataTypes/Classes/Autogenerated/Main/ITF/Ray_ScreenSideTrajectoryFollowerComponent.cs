namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_ScreenSideTrajectoryFollowerComponent : TrajectoryFollowerComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x959A0800;
	}
}

