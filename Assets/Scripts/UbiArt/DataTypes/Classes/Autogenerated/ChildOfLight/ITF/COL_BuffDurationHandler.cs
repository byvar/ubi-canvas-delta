namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffDurationHandler : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE8C260E7;
	}
}

