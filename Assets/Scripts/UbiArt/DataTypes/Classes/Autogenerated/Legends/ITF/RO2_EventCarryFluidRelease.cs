namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventCarryFluidRelease : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEB831998;
	}
}

