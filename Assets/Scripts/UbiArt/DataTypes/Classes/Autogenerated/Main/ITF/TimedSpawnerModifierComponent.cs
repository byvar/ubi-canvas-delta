namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class TimedSpawnerModifierComponent : ActorComponent {
		public TimedSpawnerData timedSpawnerData;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
		}
		public override uint? ClassCRC => 0x5DF76399;
	}
}

