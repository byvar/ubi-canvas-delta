namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class TimedSpawnerModifierComponent_Template : ActorComponent_Template {
		public TimedSpawnerData timedSpawnerData;
		public bool useInstanceValues = true;
		public bool isTriggered;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
			useInstanceValues = s.Serialize<bool>(useInstanceValues, name: "useInstanceValues");
			isTriggered = s.Serialize<bool>(isTriggered, name: "isTriggered");
		}
		public override uint? ClassCRC => 0xD49FDDEC;
	}
}

