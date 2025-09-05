namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_GameOptionsManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF63DEDA6;
	}
}

