namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIGround_ReceiveEarthquakeHitAction : Ray_AIGround_ReceiveNormalHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB9BA63C9;
	}
}

