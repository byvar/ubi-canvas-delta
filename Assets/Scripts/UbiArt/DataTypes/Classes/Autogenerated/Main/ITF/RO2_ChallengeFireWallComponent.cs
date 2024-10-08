namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_ChallengeFireWallComponent : ActorComponent {
		public Vec2d screenPosition = new Vec2d(0.5f, 0.5f);
		public Vec3d actorPosAtCheckpointSave;
		public State state;
		public bool useScreenPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.Settings.Platform == GamePlatform.Vita) {
					screenPosition = s.SerializeObject<Vec2d>(screenPosition, name: "screenPosition");
					useScreenPos = s.Serialize<bool>(useScreenPos, name: "useScreenPos");
					if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
						state = s.Serialize<State>(state, name: "state");
					}
				} else {
					screenPosition = s.SerializeObject<Vec2d>(screenPosition, name: "screenPosition");
					actorPosAtCheckpointSave = s.SerializeObject<Vec3d>(actorPosAtCheckpointSave, name: "actorPosAtCheckpointSave");
					useScreenPos = s.Serialize<bool>(useScreenPos, name: "useScreenPos");
					if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
						state = s.Serialize<State>(state, name: "state");
					}
				}
			} else {
				screenPosition = s.SerializeObject<Vec2d>(screenPosition, name: "screenPosition");
				actorPosAtCheckpointSave = s.SerializeObject<Vec3d>(actorPosAtCheckpointSave, name: "actorPosAtCheckpointSave");
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					state = s.Serialize<State>(state, name: "state");
				}
			}
		}
		public enum State {
			[Serialize("StateUnknown" )] Unknown = 0,
			[Serialize("StateDisabled")] Disabled = 1,
			[Serialize("StateFadeIn"  )] FadeIn = 2,
			[Serialize("StateEnabled" )] Enabled = 3,
			[Serialize("StateFadeOut" )] FadeOut = 4,
		}
		public override uint? ClassCRC => 0xFFC40D74;
	}
}

