namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CheatsManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x32A5FECB;
	}
}

