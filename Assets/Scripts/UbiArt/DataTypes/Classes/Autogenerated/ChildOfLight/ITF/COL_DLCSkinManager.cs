namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_DLCSkinManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3B0B0C91;
	}
}

