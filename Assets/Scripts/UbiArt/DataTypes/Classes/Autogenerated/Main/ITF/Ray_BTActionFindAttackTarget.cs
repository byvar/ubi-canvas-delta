namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BTActionFindAttackTarget : BTAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7E0F5DBF;
	}
}

