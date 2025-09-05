namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightConcentratorInteraction : COL_LightSphereInteraction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x57A2283F;
	}
}

