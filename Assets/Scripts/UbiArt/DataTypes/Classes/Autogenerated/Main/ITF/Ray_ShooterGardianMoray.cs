namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RFR)]
	public partial class Ray_ShooterGardianMoray : Ray_SnakeAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xCA652A1E;
	}
}

