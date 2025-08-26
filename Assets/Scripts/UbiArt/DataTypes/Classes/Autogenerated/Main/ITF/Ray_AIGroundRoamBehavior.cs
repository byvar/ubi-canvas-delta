namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGroundRoamBehavior : Ray_AIGroundBaseMovementBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9F7D318E;
	}
}

