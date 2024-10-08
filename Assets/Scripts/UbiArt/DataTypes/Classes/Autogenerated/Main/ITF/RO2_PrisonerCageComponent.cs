namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_PrisonerCageComponent : RO2_AIComponent {
		public EditableShape shape;
		public bool savePosOnCheckpoint = true;
		public bool canTriggerMagnet = true;
		public bool isBroken;
		public float checkpointAngle;
		public Vec2d checkpointPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					shape = s.SerializeObject<EditableShape>(shape, name: "shape");
					savePosOnCheckpoint = s.Serialize<bool>(savePosOnCheckpoint, name: "savePosOnCheckpoint");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isBroken = s.Serialize<bool>(isBroken, name: "isBroken");
					checkpointAngle = s.Serialize<float>(checkpointAngle, name: "checkpointAngle");
					checkpointPos = s.SerializeObject<Vec2d>(checkpointPos, name: "checkpointPos");
				} else if (s.Settings.Platform == GamePlatform.Vita) {
					isBroken = s.Serialize<bool>(isBroken, name: "isBroken");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					shape = s.SerializeObject<EditableShape>(shape, name: "shape");
					savePosOnCheckpoint = s.Serialize<bool>(savePosOnCheckpoint, name: "savePosOnCheckpoint");
					canTriggerMagnet = s.Serialize<bool>(canTriggerMagnet, name: "canTriggerMagnet");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isBroken = s.Serialize<bool>(isBroken, name: "isBroken");
					checkpointAngle = s.Serialize<float>(checkpointAngle, name: "checkpointAngle");
					checkpointPos = s.SerializeObject<Vec2d>(checkpointPos, name: "checkpointPos");
				}
			}
		}
		public override uint? ClassCRC => 0xC509F8EE;
	}
}

