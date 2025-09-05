namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_AnimCurveComponent : ActorComponent {
		public float animDuration;
		public Curve2D posXCurve;
		public float posXMaxValue;
		public Curve2D posYCurve;
		public float posYMaxValue;
		public Curve2D posZCurve;
		public float posZMaxValue;
		public Curve2D rotCurve;
		[Description("Max rotation value in degrees")]
		public float rotMaxValue;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animDuration = s.Serialize<float>(animDuration, name: "animDuration");
			posXCurve = s.SerializeObject<Curve2D>(posXCurve, name: "posXCurve");
			posXMaxValue = s.Serialize<float>(posXMaxValue, name: "posXMaxValue");
			posYCurve = s.SerializeObject<Curve2D>(posYCurve, name: "posYCurve");
			posYMaxValue = s.Serialize<float>(posYMaxValue, name: "posYMaxValue");
			posZCurve = s.SerializeObject<Curve2D>(posZCurve, name: "posZCurve");
			posZMaxValue = s.Serialize<float>(posZMaxValue, name: "posZMaxValue");
			rotCurve = s.SerializeObject<Curve2D>(rotCurve, name: "rotCurve");
			rotMaxValue = s.Serialize<float>(rotMaxValue, name: "rotMaxValue");
		}
		public override uint? ClassCRC => 0x797485E6;
	}
}

