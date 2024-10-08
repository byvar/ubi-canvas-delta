namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_PushButtonComponent : ActorComponent {
		public bool triggerOnStick = true;
		public bool triggerOnHit;
		public bool triggerOnDRC = true;
		public uint triggerCount;
		public uint activator;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					triggerOnStick = s.Serialize<bool>(triggerOnStick, name: "triggerOnStick");
					triggerOnHit = s.Serialize<bool>(triggerOnHit, name: "triggerOnHit");
					triggerOnDRC = s.Serialize<bool>(triggerOnDRC, name: "triggerOnDRC");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					activator = s.Serialize<uint>(activator, name: "activator");
					triggerCount = s.Serialize<uint>(triggerCount, name: "triggerCount");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					triggerOnStick = s.Serialize<bool>(triggerOnStick, name: "triggerOnStick");
					triggerOnHit = s.Serialize<bool>(triggerOnHit, name: "triggerOnHit");
					triggerOnDRC = s.Serialize<bool>(triggerOnDRC, name: "triggerOnDRC");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					triggerCount = s.Serialize<uint>(triggerCount, name: "triggerCount");
				}
			}
		}
		public override uint? ClassCRC => 0xA841D201;
	}
}

