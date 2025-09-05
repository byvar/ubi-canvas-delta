namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionBase : BTAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x279DC005;
	}
}

