namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_MicDetectorComponent : ShapeDetectorComponent {
		public float smoothFactor = 0.2f;
		public float validationDuration = 0.3f;
		public float validationRMSLevel = 0.5f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			smoothFactor = s.Serialize<float>(smoothFactor, name: "smoothFactor");
			validationDuration = s.Serialize<float>(validationDuration, name: "validationDuration");
			validationRMSLevel = s.Serialize<float>(validationRMSLevel, name: "validationRMSLevel");
		}
		public override uint? ClassCRC => 0xA94E89D7;
	}
}

