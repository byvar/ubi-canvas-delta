namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MovableObjectInteractiveComponent_Template : COL_InteractComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3C1079C4;
	}
}

