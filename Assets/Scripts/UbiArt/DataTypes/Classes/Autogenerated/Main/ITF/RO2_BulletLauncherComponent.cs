namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RAVersion)]
	public partial class RO2_BulletLauncherComponent : ActorComponent {
		public TimedSpawnerData timedSpawnerData;
		public bool startActive;
		public bool applyColorsToBullet;
		public bool useTutoOnBullet;
		public bool activePhysic;
		public bool destroyAllBulletInstanceWhenDisabled;
		public bool spawnAlwaysActive;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
					startActive = s.Serialize<bool>(startActive, name: "startActive");
					applyColorsToBullet = s.Serialize<bool>(applyColorsToBullet, name: "applyColorsToBullet");
					useTutoOnBullet = s.Serialize<bool>(useTutoOnBullet, name: "useTutoOnBullet", options: CSerializerObject.Options.BoolAsByte);
				}
			} else if (s.Settings.Game == Game.VH) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
					startActive = s.Serialize<bool>(startActive, name: "startActive");
					applyColorsToBullet = s.Serialize<bool>(applyColorsToBullet, name: "applyColorsToBullet");
					useTutoOnBullet = s.Serialize<bool>(useTutoOnBullet, name: "useTutoOnBullet");
					activePhysic = s.Serialize<bool>(activePhysic, name: "activePhysic");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
					startActive = s.Serialize<bool>(startActive, name: "startActive");
					applyColorsToBullet = s.Serialize<bool>(applyColorsToBullet, name: "applyColorsToBullet");
					useTutoOnBullet = s.Serialize<bool>(useTutoOnBullet, name: "useTutoOnBullet");
					activePhysic = s.Serialize<bool>(activePhysic, name: "activePhysic");
					destroyAllBulletInstanceWhenDisabled = s.Serialize<bool>(destroyAllBulletInstanceWhenDisabled, name: "destroyAllBulletInstanceWhenDisabled");
					spawnAlwaysActive = s.Serialize<bool>(spawnAlwaysActive, name: "spawnAlwaysActive");
				}
			}
		}
		public override uint? ClassCRC => 0xB782C5CD;
	}
}

