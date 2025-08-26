namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_FollowAbility : Ray_WM_BaseAbility {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA6664FB9;
	}
}

