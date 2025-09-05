namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffHealEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1CB3CE2E;
	}
}

