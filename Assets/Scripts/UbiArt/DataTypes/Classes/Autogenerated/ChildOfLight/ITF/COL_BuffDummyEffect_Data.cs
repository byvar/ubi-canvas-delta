namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffDummyEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x30C6F2C8;
	}
}

