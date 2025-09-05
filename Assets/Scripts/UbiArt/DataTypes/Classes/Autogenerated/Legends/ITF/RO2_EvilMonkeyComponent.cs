namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EvilMonkeyComponent : RO2_EnemyBTAIComponent {
		public float maxprojectiles;
		public CListO<RO2_EvilMonkeyComponent.ProjectileDesc> ProjectilesDescList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			maxprojectiles = s.Serialize<float>(maxprojectiles, name: "maxprojectiles");
			ProjectilesDescList = s.SerializeObject<CListO<RO2_EvilMonkeyComponent.ProjectileDesc>>(ProjectilesDescList, name: "ProjectilesDescList");
		}
		public override uint? ClassCRC => 0x546EFC39;


		[Games(GameFlags.RL)]
		public partial class ProjectileDesc : CSerializable {
			public Path ProjectilPath;
			public float TimeBeforNextProjectil;
			public char ProjectilType;

			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				ProjectilPath = s.SerializeObject<Path>(ProjectilPath, name: "ProjectilPath");
				TimeBeforNextProjectil = s.Serialize<float>(TimeBeforNextProjectil, name: "TimeBeforNextProjectil");
				ProjectilType = s.Serialize<char>(ProjectilType, name: "ProjectilType");
			}
		}
	}
}

