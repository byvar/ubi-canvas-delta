namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LocalizationConfig : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x60F311D3;
	}
}

