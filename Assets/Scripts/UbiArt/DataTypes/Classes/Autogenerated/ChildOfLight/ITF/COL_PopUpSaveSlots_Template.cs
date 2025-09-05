namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PopUpSaveSlots_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEEAD9424;
	}
}

