namespace UbiArt.ITF {
	[Games(GameFlags.RA | GameFlags.RM)]
	public partial class GFX_GridFluidEmitterFactors : CSerializable {
		public float ExternalForce;
		public float ExternalFluid;
		public float ExternalPrimitive;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				ExternalForce = s.Serialize<float>(ExternalForce, name: "ExternalForce");
				ExternalFluid = s.Serialize<float>(ExternalFluid, name: "ExternalFluid");
				ExternalPrimitive = s.Serialize<float>(ExternalPrimitive, name: "ExternalPrimitive");
			}
		}
		public override uint? ClassCRC => 0xC921B588;
	}
}

