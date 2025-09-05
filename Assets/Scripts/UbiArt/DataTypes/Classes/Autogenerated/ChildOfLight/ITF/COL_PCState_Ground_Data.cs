namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PCState_Ground_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEEE98871;
	}
}

