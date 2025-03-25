namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class CharacterDebuggerComponent_Template : ActorComponent_Template {
		public bool showCollider = true;
		public bool showTrajectory = true;
		public bool debugController = true;
		public bool debugAnim = true;
		public bool debugAnimInputs;
		public bool debugAI = true;
		public uint debugMaxTrajectory = 0x40;
		public bool debugSoundInputs;
		public bool debugActorPos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO || s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				showCollider = s.Serialize<bool>(showCollider, name: "showCollider");
				showTrajectory = s.Serialize<bool>(showTrajectory, name: "showTrajectory");
				debugController = s.Serialize<bool>(debugController, name: "debugController");
				debugAnim = s.Serialize<bool>(debugAnim, name: "debugAnim");
				debugAnimInputs = s.Serialize<bool>(debugAnimInputs, name: "debugAnimInputs");
				debugAI = s.Serialize<bool>(debugAI, name: "debugAI");
				debugMaxTrajectory = s.Serialize<uint>(debugMaxTrajectory, name: "debugMaxTrajectory");
				debugSoundInputs = s.Serialize<bool>(debugSoundInputs, name: "debugSoundInputs");
			} else {
				showCollider = s.Serialize<bool>(showCollider, name: "showCollider");
				showTrajectory = s.Serialize<bool>(showTrajectory, name: "showTrajectory");
				debugController = s.Serialize<bool>(debugController, name: "debugController");
				debugAnim = s.Serialize<bool>(debugAnim, name: "debugAnim");
				debugAnimInputs = s.Serialize<bool>(debugAnimInputs, name: "debugAnimInputs");
				debugAI = s.Serialize<bool>(debugAI, name: "debugAI");
				debugMaxTrajectory = s.Serialize<uint>(debugMaxTrajectory, name: "debugMaxTrajectory");
				debugSoundInputs = s.Serialize<bool>(debugSoundInputs, name: "debugSoundInputs");
				debugActorPos = s.Serialize<bool>(debugActorPos, name: "debugActorPos");
			}
		}
		public override uint? ClassCRC => 0x832235B6;
	}
}

