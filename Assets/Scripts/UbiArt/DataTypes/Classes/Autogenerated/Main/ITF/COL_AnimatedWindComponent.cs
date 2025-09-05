namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_AnimatedWindComponent : WindComponent {
		public Curve2D curveForceMultiplier;
		public float curveMaxValue;
		public float curveDuration;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			curveForceMultiplier = s.SerializeObject<Curve2D>(curveForceMultiplier, name: "curveForceMultiplier");
			curveMaxValue = s.Serialize<float>(curveMaxValue, name: "curveMaxValue");
			curveDuration = s.Serialize<float>(curveDuration, name: "curveDuration");
		}
		public override uint? ClassCRC => 0xB5EB6A88;
	}
}

