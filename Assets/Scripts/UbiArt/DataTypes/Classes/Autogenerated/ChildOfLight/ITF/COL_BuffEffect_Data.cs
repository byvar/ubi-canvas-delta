namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3B93D4E6;
	}
}

