namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffRemoveNegativeBuffsEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4A43F89A;
	}
}

