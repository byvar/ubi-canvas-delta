namespace UbiArt.ITF {
	[Games(GameFlags.RA | GameFlags.RM)]
	public partial class GFX_GridFluidLightingParam : CSerializable {
		public bool UseLighting;
		public float FrontLightIntensity;
		public float BackLightIntensity;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				UseLighting = s.Serialize<bool>(UseLighting, name: "UseLighting");
				FrontLightIntensity = s.Serialize<float>(FrontLightIntensity, name: "FrontLightIntensity");
				BackLightIntensity = s.Serialize<float>(BackLightIntensity, name: "BackLightIntensity");
			}
		}
		public override uint? ClassCRC => 0xEC756912;
	}
}

