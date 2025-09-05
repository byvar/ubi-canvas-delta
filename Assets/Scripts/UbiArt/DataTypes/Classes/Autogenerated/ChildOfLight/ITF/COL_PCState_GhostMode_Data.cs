namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PCState_GhostMode_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xAEF1C123;
	}
}

