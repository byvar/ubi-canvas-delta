namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_BreakableStackManagerAIComponent_Template : RO2_AIComponent_Template {
		public float timeShakeBeforeFall = 0.3f;
		public float countDownHit = 0.5f;
		public float gravityBallistics = -15f;
		public float timeExpulse = 3f;
		public Path atlasPath;
		public Path atlasParticlesPath;
		public GFXMaterialSerializable atlasMaterial;
		public GFXMaterialSerializable atlasParticlesMaterial;
		public float edgeSize = 1f;
		public uint faction2;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			timeShakeBeforeFall = s.Serialize<float>(timeShakeBeforeFall, name: "timeShakeBeforeFall");
			countDownHit = s.Serialize<float>(countDownHit, name: "countDownHit");
			gravityBallistics = s.Serialize<float>(gravityBallistics, name: "gravityBallistics");
			timeExpulse = s.Serialize<float>(timeExpulse, name: "timeExpulse");
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				atlasPath = s.SerializeObject<Path>(atlasPath, name: "atlasPath");
				atlasParticlesPath = s.SerializeObject<Path>(atlasParticlesPath, name: "atlasParticlesPath");
			}
			atlasMaterial = s.SerializeObject<GFXMaterialSerializable>(atlasMaterial, name: "atlasMaterial");
			atlasParticlesMaterial = s.SerializeObject<GFXMaterialSerializable>(atlasParticlesMaterial, name: "atlasParticlesMaterial");
			edgeSize = s.Serialize<float>(edgeSize, name: "edgeSize");
			faction2 = s.Serialize<uint>(faction2, name: "faction");
		}
		public override uint? ClassCRC => 0x189DAE6B;
	}
}

