namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_ActorsAssetsConfig : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6D9EBCB8;
	}
}

