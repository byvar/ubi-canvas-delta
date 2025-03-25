namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_ElCrapoBehaviorComponent_Template : RO2_AIComponent_Template {
		public float jumpSpeed = 15;
		public float bounceSpeed = 15;
		public float bumperSpeed = 25;
		public float maxSpeed = 15;
		public float airControl = 1;
		public float fullSpeedThreshold = 0.8f;
		public float dangerousContactHeight = 0.5f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			jumpSpeed = s.Serialize<float>(jumpSpeed, name: "jumpSpeed");
			bounceSpeed = s.Serialize<float>(bounceSpeed, name: "bounceSpeed");
			bumperSpeed = s.Serialize<float>(bumperSpeed, name: "bumperSpeed");
			maxSpeed = s.Serialize<float>(maxSpeed, name: "maxSpeed");
			airControl = s.Serialize<float>(airControl, name: "airControl");
			fullSpeedThreshold = s.Serialize<float>(fullSpeedThreshold, name: "fullSpeedThreshold");
			dangerousContactHeight = s.Serialize<float>(dangerousContactHeight, name: "dangerousContactHeight");
		}
		public override uint? ClassCRC => 0xFBCC8031;
	}
}

