namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_HeartShield : Ray_PowerUpDisplay {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFAE3A48D;
	}
}

