namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BTActionWalkToTargetSprintWithPlayer : BTActionWalkToTarget {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA39BA75C;
	}
}

