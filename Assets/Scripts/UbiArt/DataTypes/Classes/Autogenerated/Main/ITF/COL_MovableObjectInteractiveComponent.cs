namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MovableObjectInteractiveComponent : COL_InteractComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x694F7B24;
	}
}

