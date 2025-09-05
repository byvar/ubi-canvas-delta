namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_PinkMonkeyComponent : ActorComponent {
		public CListO<RO2_PinkMonkeyComponent.ProjectileDesc> ProjectilesDescList;
		public float MaxBullet;
		public float TimerBeforDown;
		public float MaxYBullet;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			ProjectilesDescList = s.SerializeObject<CListO<RO2_PinkMonkeyComponent.ProjectileDesc>>(ProjectilesDescList, name: "ProjectilesDescList");
			MaxBullet = s.Serialize<float>(MaxBullet, name: "MaxBullet");
			TimerBeforDown = s.Serialize<float>(TimerBeforDown, name: "TimerBeforDown");
			MaxYBullet = s.Serialize<float>(MaxYBullet, name: "MaxYBullet");
		}
		public override uint? ClassCRC => 0x86865CB8;


		[Games(GameFlags.RL)]
		public partial class ProjectileDesc : CSerializable {
			public Path ProjectilPath;
			public float TimeBeforNextProjectil;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				ProjectilPath = s.SerializeObject<Path>(ProjectilPath, name: "ProjectilPath");
				TimeBeforNextProjectil = s.Serialize<float>(TimeBeforNextProjectil, name: "TimeBeforNextProjectil");
			}
		}
	}
}

