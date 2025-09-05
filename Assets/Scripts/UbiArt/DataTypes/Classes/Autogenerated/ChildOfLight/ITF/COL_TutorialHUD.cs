namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TutorialHUD : UIMenuBasic {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x57454532;
	}
}

