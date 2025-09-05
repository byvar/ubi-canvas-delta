namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InventoryGenericItemDrop_Data : COL_InventoryItemDrop_Data {
		public uint amount;
		public StringID itemID;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			amount = s.Serialize<uint>(amount, name: "amount");
			itemID = s.SerializeObject<StringID>(itemID, name: "itemID");
		}
		public override uint? ClassCRC => 0xA01B6831;
	}
}

