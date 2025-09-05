namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PlayerManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x78587AE0;
	}
}

