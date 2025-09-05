namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CoopDrawActionController : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8B4DD0B7;
	}
}

