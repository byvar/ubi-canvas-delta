namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_BossNodeComponent : ActorComponent {
		public float playerSpeed = 10f;
		public StringID music;
		public StringID musicPart;
		public bool cameraFollowPlayer;
		public FireMode fireMode;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				playerSpeed = s.Serialize<float>(playerSpeed, name: "playerSpeed");
				music = s.SerializeObject<StringID>(music, name: "music");
				musicPart = s.SerializeObject<StringID>(musicPart, name: "musicPart");
				cameraFollowPlayer = s.Serialize<bool>(cameraFollowPlayer, name: "cameraFollowPlayer");
				fireMode = s.Serialize<FireMode>(fireMode, name: "fireMode");
			}
		}
		public enum FireMode {
			[Serialize("FireMode_Previous" )] Previous = 0,
			[Serialize("FireMode_Automatic")] Automatic = 1,
			[Serialize("FireMode_On"       )] On = 2,
			[Serialize("FireMode_Off"      )] Off = 3,
		}
		public override uint? ClassCRC => 0x5355C0B5;
	}
}

