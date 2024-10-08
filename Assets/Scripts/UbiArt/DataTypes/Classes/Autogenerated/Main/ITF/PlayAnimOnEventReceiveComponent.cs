namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class PlayAnimOnEventReceiveComponent : ActorComponent {
		public bool disabledAfterEvent;
		public bool playingEventCheckpointSave;
		public int eventIdlePlayed;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					disabledAfterEvent = s.Serialize<bool>(disabledAfterEvent, name: "disabledAfterEvent");
					playingEventCheckpointSave = s.Serialize<bool>(playingEventCheckpointSave, name: "playingEventCheckpointSave");
					eventIdlePlayed = s.Serialize<int>(eventIdlePlayed, name: "eventIdlePlayed");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					disabledAfterEvent = s.Serialize<bool>(disabledAfterEvent, name: "disabledAfterEvent");
					playingEventCheckpointSave = s.Serialize<bool>(playingEventCheckpointSave, name: "playingEventCheckpointSave");
				}
			}
		}
		public override uint? ClassCRC => 0xAA6D656B;
	}
}

