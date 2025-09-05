namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_UIEventRequestGameOverMenuExit : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xCB6873AE;
	}
}

