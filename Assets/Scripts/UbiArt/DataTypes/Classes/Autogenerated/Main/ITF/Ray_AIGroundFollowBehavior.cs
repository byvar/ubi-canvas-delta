namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGroundFollowBehavior : Ray_AIGroundRoamBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEFBD7E37;
	}
}

