namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIFollowBezierCurveAction : AIAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0BE5B657;
	}
}

