namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_ConditionalTriggerComponent : TriggerComponent {
		// To use: link objects to this with tags "Dead", "Enable", "Trigger" or "Open" and tag value 0 or 1 (positive or negative)
		// Then link this to whatever you want to trigger
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9EAA6F47;
	}
}

