namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionBase_Template : BTAction_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBD8B6BEB;
	}
}

