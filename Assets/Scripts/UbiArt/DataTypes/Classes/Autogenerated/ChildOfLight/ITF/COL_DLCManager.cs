namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_DLCManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x23BF7E7D;
	}
}

