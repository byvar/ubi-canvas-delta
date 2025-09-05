namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_UIEventShowQuestValidation : COL_UIEventShowPopUp {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8BEF9FB0;
	}
}

