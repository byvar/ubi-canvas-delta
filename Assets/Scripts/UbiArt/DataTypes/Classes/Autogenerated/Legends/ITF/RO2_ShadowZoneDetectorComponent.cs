namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShadowZoneDetectorComponent : ShapeDetectorComponent {
		public bool detectInLight;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			detectInLight = s.Serialize<bool>(detectInLight, name: "detectInLight", options: CSerializerObject.Options.BoolAsByte);
		}
		public override uint? ClassCRC => 0x952E1E18;
	}
}

