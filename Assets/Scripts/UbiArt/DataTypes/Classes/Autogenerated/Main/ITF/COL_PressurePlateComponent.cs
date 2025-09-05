namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PressurePlateComponent : COL_InteractComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4B00957F;
	}
}

