namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffParalyzeEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x79870733;
	}
}

