namespace UbiArt.ITF {
	[Games(GameFlags.RA)]
	public partial class PlayerLevelInfos : CSerializable {
		public Path actorPath;
		public Path actorPath3D;
		public uint lumsReward;
		public SmartLocId name;
		public SmartLocId nameInBloomberg;
		public uint uint_vita;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			actorPath = s.SerializeObject<Path>(actorPath, name: "actorPath");
			actorPath3D = s.SerializeObject<Path>(actorPath3D, name: "actorPath3D");
			lumsReward = s.Serialize<uint>(lumsReward, name: "lumsReward");
			if (s.Settings.Platform == GamePlatform.Vita) {
				uint_vita = s.Serialize<uint>(uint_vita, name: "uint_vita");
			}
			name = s.SerializeObject<SmartLocId>(name, name: "name");
			nameInBloomberg = s.SerializeObject<SmartLocId>(nameInBloomberg, name: "nameInBloomberg");
		}
	}
}

