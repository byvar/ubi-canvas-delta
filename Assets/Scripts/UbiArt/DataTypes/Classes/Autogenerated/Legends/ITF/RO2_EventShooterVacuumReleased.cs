namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventShooterVacuumReleased : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x73D54BA6;
	}
}

