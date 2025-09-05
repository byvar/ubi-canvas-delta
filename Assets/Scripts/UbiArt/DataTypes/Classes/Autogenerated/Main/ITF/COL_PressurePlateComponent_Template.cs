namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PressurePlateComponent_Template : COL_InteractComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2C70672A;
	}
}

