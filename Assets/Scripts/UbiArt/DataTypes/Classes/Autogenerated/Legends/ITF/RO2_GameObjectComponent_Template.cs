namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_GameObjectComponent_Template : ActorComponent_Template {
		public uint networkDT = uint.MaxValue;
		public uint networkUpdateSleepTime = uint.MaxValue;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if(this is RO2_AdversarialModeComponent_Template) return;
			networkDT = s.Serialize<uint>(networkDT, name: "networkDT");
			networkUpdateSleepTime = s.Serialize<uint>(networkUpdateSleepTime, name: "networkUpdateSleepTime");
		}
		public override uint? ClassCRC => 0xA26D279A;
	}
}

