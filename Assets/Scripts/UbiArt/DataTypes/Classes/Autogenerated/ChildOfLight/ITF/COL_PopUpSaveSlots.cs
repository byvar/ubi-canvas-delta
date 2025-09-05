namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PopUpSaveSlots : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x09C6D87A;
	}
}

