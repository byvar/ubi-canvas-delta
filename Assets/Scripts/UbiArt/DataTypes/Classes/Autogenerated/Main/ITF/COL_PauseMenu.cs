namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PauseMenu : UIMenuBasic {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4F20DAAE;
	}
}

