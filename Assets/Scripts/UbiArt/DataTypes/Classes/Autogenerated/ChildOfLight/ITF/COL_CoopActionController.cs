namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CoopActionController : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x73E705C2;
	}
}

