namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRoamingVertical : COL_BTActionBase {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xED74F160;
	}
}

