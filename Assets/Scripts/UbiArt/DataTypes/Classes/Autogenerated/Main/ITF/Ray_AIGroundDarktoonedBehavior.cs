namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGroundDarktoonedBehavior : Ray_AIGroundRoamBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF4E5D5E6;
	}
}

