namespace UbiArt.ITF {
	[Games(GameFlags.RFR)]
	public partial class Ray_GuardianNodeAIComponent_Template : ActorComponent_Template {
		public int int__0 = 1;
		public uint prevNodeCount;
		public float sampleLength = 0.1f;
		public float acceleration = 1f;
		public Vec2d targetEvaluationOffset;
		public float speedMultiplierMinDistance = 5f;
		public float speedMultiplierMaxDistance = 10f;
		public float speedMultiplierMinValue = 0.7f;
		public float speedMultiplierMaxValue = 1.3f;
		public float speedMultiplierAcceleration = 1f;
		public float speedMultiplierDeceleration = 1f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			int__0 = s.Serialize<int>(int__0, name: "int__0");
			prevNodeCount = s.Serialize<uint>(prevNodeCount, name: "prevNodeCount");
			sampleLength = s.Serialize<float>(sampleLength, name: "sampleLength");
			acceleration = s.Serialize<float>(acceleration, name: "acceleration");
			targetEvaluationOffset = s.SerializeObject<Vec2d>(targetEvaluationOffset, name: "targetEvaluationOffset");
			speedMultiplierMinDistance = s.Serialize<float>(speedMultiplierMinDistance, name: "speedMultiplierMinDistance");
			speedMultiplierMaxDistance = s.Serialize<float>(speedMultiplierMaxDistance, name: "speedMultiplierMaxDistance");
			speedMultiplierMinValue = s.Serialize<float>(speedMultiplierMinValue, name: "speedMultiplierMinValue");
			speedMultiplierMaxValue = s.Serialize<float>(speedMultiplierMaxValue, name: "speedMultiplierMaxValue");
			speedMultiplierAcceleration = s.Serialize<float>(speedMultiplierAcceleration, name: "speedMultiplierAcceleration");
			speedMultiplierDeceleration = s.Serialize<float>(speedMultiplierDeceleration, name: "speedMultiplierDeceleration");
		}
		public override uint? ClassCRC => 0x629D749F;
	}
}

