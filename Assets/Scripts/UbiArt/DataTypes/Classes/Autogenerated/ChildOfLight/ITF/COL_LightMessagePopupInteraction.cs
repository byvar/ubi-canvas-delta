namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightMessagePopupInteraction : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xDBAD9EB8;
	}
}

