namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InventoryItem : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE0E5E754;
	}
}

