namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRoaming : COL_BTActionBase {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x94E7329B;
	}
}

