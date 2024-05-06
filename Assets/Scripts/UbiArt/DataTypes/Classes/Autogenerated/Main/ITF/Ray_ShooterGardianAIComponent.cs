namespace UbiArt.ITF {
	[Games(GameFlags.RFR)]
	public partial class Ray_ShooterGardianAIComponent : Ray_SimpleAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x03FC9CC5;
	}
}

