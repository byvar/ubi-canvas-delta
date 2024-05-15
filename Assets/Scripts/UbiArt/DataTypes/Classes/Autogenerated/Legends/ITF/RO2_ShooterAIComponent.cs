namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShooterAIComponent : RO2_AIComponent {
		public Vec3d beforeCamRelativeInitialPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				beforeCamRelativeInitialPos = s.SerializeObject<Vec3d>(beforeCamRelativeInitialPos, name: "beforeCamRelativeInitialPos");
			}
		}
		public override uint? ClassCRC => 0x0C7AC209;
	}
}

