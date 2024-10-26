namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_MusicalBossComponent : ActorComponent {
		public float bpm = 180.18f;
		public float triggerDistance = 6;
		public float volume = -2;
		public float fadeAfterCheckpoint;
		public uint currentNodeIndex;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				bpm = s.Serialize<float>(bpm, name: "bpm");
				triggerDistance = s.Serialize<float>(triggerDistance, name: "triggerDistance");
				volume = s.Serialize<float>(volume, name: "volume");
				fadeAfterCheckpoint = s.Serialize<float>(fadeAfterCheckpoint, name: "fadeAfterCheckpoint");
			}
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				currentNodeIndex = s.Serialize<uint>(currentNodeIndex, name: "currentNodeIndex");
			}
		}
		public override uint? ClassCRC => 0x0C488105;
	}
}

