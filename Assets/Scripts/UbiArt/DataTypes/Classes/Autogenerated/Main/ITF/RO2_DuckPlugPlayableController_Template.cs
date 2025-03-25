namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_DuckPlugPlayableController_Template : RO2_ActorPlugTransfoController_Template {
		public float walkSpeed = 3;
		public float runSpeed = 7;
		public float flySpeed = 10;
		public float wingBoost = 0.5f;
		public float wingXBoost = 1.05f;
		public float fallBreak = 0.5f;
		public float bounceImpulse = 500;
		public float blendSpeedFactor = 1;
		public float groundSpeedFactor;
		public float maxForce = 100;
		public float breakingThreshold = -5;
		public float speedReduce = -1.5f;
		public float speedFactorWhenHit = 0.2f;
		public float fallFriction = 5;
		public float deadSoulSpeed = 15;
		public float deadSoulControl = 1.5f;
		public float deadSoulForcedMove = 1.2f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			walkSpeed = s.Serialize<float>(walkSpeed, name: "walkSpeed");
			runSpeed = s.Serialize<float>(runSpeed, name: "runSpeed");
			flySpeed = s.Serialize<float>(flySpeed, name: "flySpeed");
			wingBoost = s.Serialize<float>(wingBoost, name: "wingBoost");
			wingXBoost = s.Serialize<float>(wingXBoost, name: "wingXBoost");
			fallBreak = s.Serialize<float>(fallBreak, name: "fallBreak");
			bounceImpulse = s.Serialize<float>(bounceImpulse, name: "bounceImpulse");
			blendSpeedFactor = s.Serialize<float>(blendSpeedFactor, name: "blendSpeedFactor");
			groundSpeedFactor = s.Serialize<float>(groundSpeedFactor, name: "groundSpeedFactor");
			maxForce = s.Serialize<float>(maxForce, name: "maxForce");
			breakingThreshold = s.Serialize<float>(breakingThreshold, name: "breakingThreshold");
			speedReduce = s.Serialize<float>(speedReduce, name: "speedReduce");
			speedFactorWhenHit = s.Serialize<float>(speedFactorWhenHit, name: "speedFactorWhenHit");
			fallFriction = s.Serialize<float>(fallFriction, name: "fallFriction");
			deadSoulSpeed = s.Serialize<float>(deadSoulSpeed, name: "deadSoulSpeed");
			deadSoulControl = s.Serialize<float>(deadSoulControl, name: "deadSoulControl");
			deadSoulForcedMove = s.Serialize<float>(deadSoulForcedMove, name: "deadSoulForcedMove");
		}
		public override uint? ClassCRC => 0xCBD9D4E2;
	}
}

