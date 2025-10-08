namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class MusicControllerComponent_Template : ActorComponent_Template {
		public ProceduralInputData musicVolume;
		public uint metronomeType;
		public CListO<InputDesc> inputs;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			musicVolume = s.SerializeObject<ProceduralInputData>(musicVolume, name: "musicVolume");
			metronomeType = s.Serialize<uint>(metronomeType, name: "metronomeType");
			inputs = s.SerializeObject<CListO<InputDesc>>(inputs, name: "inputs");
		}
		public override uint? ClassCRC => 0xFBE85770;
	}
}

