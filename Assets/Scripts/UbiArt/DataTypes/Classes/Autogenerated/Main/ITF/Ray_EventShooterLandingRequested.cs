namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventShooterLandingRequested : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1D5F4853;
	}
}

