namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightSphereInteraction : COL_ObjectInteraction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFD7CAECC;
	}
}

