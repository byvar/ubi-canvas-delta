namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_ToadPlugPlayableController : ActorPlugPlayableController {
		public float tweakJumpImpulse = 900;
		public float tweakBoostedJumpImpulse = 1500;
		public float tweakBoostTolerance = 0.25f;
		public Vec2d tweakAirControlSpeed = new Vec2d(8, 0);
		public float tweakAirControlBlendSpeed = 5;
		public float tweakAirControlMaxForce = 300;
		public float tweakPhysGravityMultiplier = 0.6f;
		public float minUnPlugJumpHeightFactor = 0.5f;
		public float maxUnPlugJumpHeightFactor = 0.75f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				tweakJumpImpulse = s.Serialize<float>(tweakJumpImpulse, name: "tweakJumpImpulse");
				tweakBoostedJumpImpulse = s.Serialize<float>(tweakBoostedJumpImpulse, name: "tweakBoostedJumpImpulse");
				tweakBoostTolerance = s.Serialize<float>(tweakBoostTolerance, name: "tweakBoostTolerance");
				tweakAirControlSpeed = s.SerializeObject<Vec2d>(tweakAirControlSpeed, name: "tweakAirControlSpeed");
				tweakAirControlBlendSpeed = s.Serialize<float>(tweakAirControlBlendSpeed, name: "tweakAirControlBlendSpeed");
				tweakAirControlMaxForce = s.Serialize<float>(tweakAirControlMaxForce, name: "tweakAirControlMaxForce");
				tweakPhysGravityMultiplier = s.Serialize<float>(tweakPhysGravityMultiplier, name: "tweakPhysGravityMultiplier");
				minUnPlugJumpHeightFactor = s.Serialize<float>(minUnPlugJumpHeightFactor, name: "minUnPlugJumpHeightFactor");
				maxUnPlugJumpHeightFactor = s.Serialize<float>(maxUnPlugJumpHeightFactor, name: "maxUnPlugJumpHeightFactor");
			}
		}
		public override uint? ClassCRC => 0x869BA922;
	}
}

