namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_FeedbackManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x115A52E3;
	}
}

