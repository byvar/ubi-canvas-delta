namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InventoryItemPack : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD3D3A620;
	}
}

