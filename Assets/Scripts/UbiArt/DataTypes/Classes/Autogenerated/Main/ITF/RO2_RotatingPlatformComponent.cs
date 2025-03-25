namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_RotatingPlatformComponent : ActorComponent {
		public bool rotating;
		public Angle destinationAngle;
		public float blendTime = 1;
		public float angularSpeed = 1.5f;
		public bool useClockwiseRotation = true;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				rotating = s.Serialize<bool>(rotating, name: "rotating");
			}
			destinationAngle = s.SerializeObject<Angle>(destinationAngle, name: "destinationAngle");
			blendTime = s.Serialize<float>(blendTime, name: "blendTime");
			angularSpeed = s.Serialize<float>(angularSpeed, name: "angularSpeed");
			useClockwiseRotation = s.Serialize<bool>(useClockwiseRotation, name: "useClockwiseRotation");
		}
		public override uint? ClassCRC => 0xCF1C34AF;
	}
}

