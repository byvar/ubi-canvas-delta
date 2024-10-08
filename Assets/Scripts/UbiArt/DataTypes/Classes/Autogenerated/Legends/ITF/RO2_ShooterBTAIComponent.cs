namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShooterBTAIComponent : RO2_EnemyBTAIComponent {
		public Vec3d beforeCamRelativeInitialPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				beforeCamRelativeInitialPos = s.SerializeObject<Vec3d>(beforeCamRelativeInitialPos, name: "beforeCamRelativeInitialPos");
			}
		}
		public override uint? ClassCRC => 0xAEAE137A;
	}
}

