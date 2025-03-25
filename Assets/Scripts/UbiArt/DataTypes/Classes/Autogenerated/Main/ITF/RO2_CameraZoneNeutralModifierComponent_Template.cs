namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_CameraZoneNeutralModifierComponent_Template : ActorComponent_Template {
		public Vec2d zoneNeutral;
		public float swipeSpeed;
		public float swipeToleranceTime = 0.5f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			zoneNeutral = s.SerializeObject<Vec2d>(zoneNeutral, name: "zoneNeutral");
			swipeSpeed = s.Serialize<float>(swipeSpeed, name: "swipeSpeed");
			swipeToleranceTime = s.Serialize<float>(swipeToleranceTime, name: "swipeToleranceTime");
		}
		public override uint? ClassCRC => 0xBE62EA71;
	}
}

