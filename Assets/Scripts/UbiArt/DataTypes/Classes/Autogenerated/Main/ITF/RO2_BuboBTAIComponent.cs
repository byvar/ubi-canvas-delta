namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_BuboBTAIComponent : BTAIComponent {
		public bool crushable = true;
		public bool triggerActivator;
		public bool delayTrigger;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				crushable = s.Serialize<bool>(crushable, name: "crushable");
				triggerActivator = s.Serialize<bool>(triggerActivator, name: "triggerActivator");
				delayTrigger = s.Serialize<bool>(delayTrigger, name: "delayTrigger");
			}
		}
		public override uint? ClassCRC => 0x80FD5DF4;
	}
}

