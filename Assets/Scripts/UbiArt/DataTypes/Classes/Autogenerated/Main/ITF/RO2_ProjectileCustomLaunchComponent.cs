namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_ProjectileCustomLaunchComponent : ActorComponent {
		public CListO<RO2_BTActionThrowObject_Tools.LaunchData> customLaunchData;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				customLaunchData = s.SerializeObject<CListO<RO2_BTActionThrowObject_Tools.LaunchData>>(customLaunchData, name: "customLaunchData");
			}
		}
		public override uint? ClassCRC => 0x0E5D219B;
	}
}

