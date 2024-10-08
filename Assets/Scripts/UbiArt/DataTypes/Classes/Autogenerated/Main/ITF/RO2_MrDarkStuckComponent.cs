namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_MrDarkStuckComponent : ActorComponent {
		public bool disabledAfterEvent;
		public bool playingEventCheckpointSave;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				disabledAfterEvent = s.Serialize<bool>(disabledAfterEvent, name: "disabledAfterEvent");
				playingEventCheckpointSave = s.Serialize<bool>(playingEventCheckpointSave, name: "playingEventCheckpointSave");
			}
		}
		public override uint? ClassCRC => 0x428CD65D;
	}
}

