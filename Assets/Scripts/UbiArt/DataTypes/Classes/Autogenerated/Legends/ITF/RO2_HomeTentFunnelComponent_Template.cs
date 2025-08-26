namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_HomeTentFunnelComponent_Template : RO2_HomeComponent_Template {
		public PhysShapePolygon enterShape;
		public Vec2d enterPoint0;
		public Vec2d enterPoint1;
		public float playerSpeedMultiplier = 1f;
		public float travelSpeed = 1f;
		public float scale = 0.5f;
		public float catchCooldown = 0.1f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			enterShape = s.SerializeObject<PhysShapePolygon>(enterShape, name: "enterShape");
			enterPoint0 = s.SerializeObject<Vec2d>(enterPoint0, name: "enterPoint0");
			enterPoint1 = s.SerializeObject<Vec2d>(enterPoint1, name: "enterPoint1");
			playerSpeedMultiplier = s.Serialize<float>(playerSpeedMultiplier, name: "playerSpeedMultiplier");
			travelSpeed = s.Serialize<float>(travelSpeed, name: "travelSpeed");
			scale = s.Serialize<float>(scale, name: "scale");
			catchCooldown = s.Serialize<float>(catchCooldown, name: "catchCooldown");
		}
		public override uint? ClassCRC => 0x8616D48B;
	}
}

