namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BTActionReceiveHit : BTAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x976AC775;
	}
}

