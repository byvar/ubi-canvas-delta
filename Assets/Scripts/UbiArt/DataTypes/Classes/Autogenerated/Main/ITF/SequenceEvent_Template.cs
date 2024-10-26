namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RFR | GameFlags.LegendsAndUp)]
	public partial class SequenceEvent_Template : CSerializable {
		public int StartFrame;
		public int Offset;
		public int Duration = 1;
		public uint TrackLine = 1;
		public string Channel = "";
		public int Selected = -1;
		public bool DisabledForTesting;
		public uint uid = uint.MaxValue;
		public event_mode EventMode;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				StartFrame = s.Serialize<int>(StartFrame, name: "StartFrame");
				Offset = s.Serialize<int>(Offset, name: "Offset");
				Duration = s.Serialize<int>(Duration, name: "Duration");
				TrackLine = s.Serialize<uint>(TrackLine, name: "TrackLine");
				Channel = s.Serialize<string>(Channel, name: "Channel");
				if (s.HasFlags(SerializeFlags.Editor)) {
					Selected = s.Serialize<int>(Selected, name: "Selected");
					DisabledForTesting = s.Serialize<bool>(DisabledForTesting, name: "DisabledForTesting");
				}
			} else {
				StartFrame = s.Serialize<int>(StartFrame, name: "StartFrame");
				Offset = s.Serialize<int>(Offset, name: "Offset");
				Duration = s.Serialize<int>(Duration, name: "Duration");
				TrackLine = s.Serialize<uint>(TrackLine, name: "TrackLine");
				Channel = s.Serialize<string>(Channel, name: "Channel");
				if (s.HasFlags(SerializeFlags.Editor)) {
					Selected = s.Serialize<int>(Selected, name: "Selected");
					DisabledForTesting = s.Serialize<bool>(DisabledForTesting, name: "DisabledForTesting");
					uid = s.Serialize<uint>(uid, name: "uid");
				}
				EventMode = s.Serialize<event_mode>(EventMode, name: "EventMode");
			}
		}
		public enum event_mode {
			[Serialize("event_mode_default"      )] _default = 0,
			[Serialize("event_mode_sequence_only")] sequence_only = 1,
			[Serialize("event_mode_editor_only"  )] editor_only = 2,
		}
		public override uint? ClassCRC => 0xFB5A38F1;
	}
}

