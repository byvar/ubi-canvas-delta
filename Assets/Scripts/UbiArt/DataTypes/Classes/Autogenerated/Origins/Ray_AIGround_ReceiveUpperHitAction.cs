namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGround_ReceiveUpperHitAction : Ray_AIGroundReceiveHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xCEAF9AB7;
	}
}

