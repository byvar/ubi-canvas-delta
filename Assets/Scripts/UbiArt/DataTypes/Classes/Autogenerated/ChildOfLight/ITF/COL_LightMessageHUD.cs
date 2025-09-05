namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightMessageHUD : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2C90E5B0;
	}
}

