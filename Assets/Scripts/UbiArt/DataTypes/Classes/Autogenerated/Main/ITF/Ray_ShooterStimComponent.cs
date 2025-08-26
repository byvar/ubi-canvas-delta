namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RFR)]
	public partial class Ray_ShooterStimComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3EDB59ED;
	}
}

