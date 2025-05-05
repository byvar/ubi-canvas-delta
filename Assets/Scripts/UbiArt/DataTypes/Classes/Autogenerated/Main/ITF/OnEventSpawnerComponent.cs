namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class OnEventSpawnerComponent : ActorComponent {
		public Path actorToSpawn;
		public CArrayO<Generic<Event>> onSpawnEvents;
		public bool autoStart;
		public EventSpawn spawnDataAutoStart;
		public Enum_SpawnObjectRef SpawnObjectRef;
		public Enum_SpawnPosRef SpawnPosRef;
		public uint KEY;
		public CArrayO<StringID> BoneGroups;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.VH) {
				actorToSpawn = s.SerializeObject<Path>(actorToSpawn, name: "actorToSpawn");
				onSpawnEvents = s.SerializeObject<CArrayO<Generic<Event>>>(onSpawnEvents, name: "onSpawnEvents");
				autoStart = s.Serialize<bool>(autoStart, name: "autoStart");
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					spawnDataAutoStart = s.SerializeObject<EventSpawn>(spawnDataAutoStart, name: "spawnDataAutoStart");
				}
			} else {
				actorToSpawn = s.SerializeObject<Path>(actorToSpawn, name: "actorToSpawn");
				onSpawnEvents = s.SerializeObject<CArrayO<Generic<Event>>>(onSpawnEvents, name: "onSpawnEvents");
				autoStart = s.Serialize<bool>(autoStart, name: "autoStart");
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					spawnDataAutoStart = s.SerializeObject<EventSpawn>(spawnDataAutoStart, name: "spawnDataAutoStart");
				}
				SpawnObjectRef = s.Serialize<Enum_SpawnObjectRef>(SpawnObjectRef, name: "SpawnObjectRef");
				SpawnPosRef = s.Serialize<Enum_SpawnPosRef>(SpawnPosRef, name: "SpawnPosRef");
				KEY = s.Serialize<uint>(KEY, name: "KEY");
				BoneGroups = s.SerializeObject<CArrayO<StringID>>(BoneGroups, name: "BoneGroups");
				KEY = s.Serialize<uint>(KEY, name: "KEY");
				BoneGroups = s.SerializeObject<CArrayO<StringID>>(BoneGroups, name: "BoneGroups");
			}
		}
		public enum Enum_SpawnObjectRef {
			[Serialize("eSelf"     )] eSelf = 0,
			[Serialize("eLinkChild")] eLinkChild = 1,
		}
		public enum Enum_SpawnPosRef {
			[Serialize("eObjectPos")] eObjectPos = 0,
			[Serialize("eBonePos"  )] eBonePos = 1,
		}
		public override uint? ClassCRC => 0xCDBC1FC4;
	}
}

