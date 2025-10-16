namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_MusicalGhostComponent_Template : ActorComponent_Template {
		public float minSpeed = 5;
		public float maxSpeed = 10;
		public float randomSpeedMultiplier = 0.1f;
		public float acceleration = 10;
		public AngleAmount minAngularSpeed = 1.047198f;
		public AngleAmount maxAngularSpeed = 1.570796f;
		public AngleAmount angularAcceleration = 1.570796f;
		public float initialSpeedBoostMultiplier;
		public float initialSpeedBoostDuration;
		public float fleeSpeedMultiplier = 2;
		public float fleeZSpeed;
		public float targetChangeTimeMin = 1;
		public float targetChangeTimeMax = 3;
		public float targetAnticipationMultiplier = 1;
		public CListO<AABB> targetBoxes;
		public PerlinNoise_Template m_targetOffsetNoise;
		public float targetBoxChangeProbability = 0.3f;
		public StringID regionId = "Jellyfish";
		public StringID avoidRegionId = "Jellyfish-avoid";
		public float raycastDepth = 3;
		public float goBackToRegionTimeLimit = 2;
		public bool drawDebug;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			minSpeed = s.Serialize<float>(minSpeed, name: "minSpeed");
			maxSpeed = s.Serialize<float>(maxSpeed, name: "maxSpeed");
			randomSpeedMultiplier = s.Serialize<float>(randomSpeedMultiplier, name: "randomSpeedMultiplier");
			acceleration = s.Serialize<float>(acceleration, name: "acceleration");
			minAngularSpeed = s.SerializeObject<AngleAmount>(minAngularSpeed, name: "minAngularSpeed");
			maxAngularSpeed = s.SerializeObject<AngleAmount>(maxAngularSpeed, name: "maxAngularSpeed");
			angularAcceleration = s.SerializeObject<AngleAmount>(angularAcceleration, name: "angularAcceleration");
			initialSpeedBoostMultiplier = s.Serialize<float>(initialSpeedBoostMultiplier, name: "initialSpeedBoostMultiplier");
			initialSpeedBoostDuration = s.Serialize<float>(initialSpeedBoostDuration, name: "initialSpeedBoostDuration");
			fleeSpeedMultiplier = s.Serialize<float>(fleeSpeedMultiplier, name: "fleeSpeedMultiplier");
			fleeZSpeed = s.Serialize<float>(fleeZSpeed, name: "fleeZSpeed");
			targetChangeTimeMin = s.Serialize<float>(targetChangeTimeMin, name: "targetChangeTimeMin");
			targetChangeTimeMax = s.Serialize<float>(targetChangeTimeMax, name: "targetChangeTimeMax");
			targetAnticipationMultiplier = s.Serialize<float>(targetAnticipationMultiplier, name: "targetAnticipationMultiplier");
			targetBoxes = s.SerializeObject<CListO<AABB>>(targetBoxes, name: "targetBoxes");
			m_targetOffsetNoise = s.SerializeObject<PerlinNoise_Template>(m_targetOffsetNoise, name: "m_targetOffsetNoise");
			targetBoxChangeProbability = s.Serialize<float>(targetBoxChangeProbability, name: "targetBoxChangeProbability");
			regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
			avoidRegionId = s.SerializeObject<StringID>(avoidRegionId, name: "avoidRegionId");
			raycastDepth = s.Serialize<float>(raycastDepth, name: "raycastDepth");
			goBackToRegionTimeLimit = s.Serialize<float>(goBackToRegionTimeLimit, name: "goBackToRegionTimeLimit");
			drawDebug = s.Serialize<bool>(drawDebug, name: "drawDebug");
		}
		public override uint? ClassCRC => 0xE5B638C1;
	}
}

