namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffNoDuration_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3E736F02;
	}
}

