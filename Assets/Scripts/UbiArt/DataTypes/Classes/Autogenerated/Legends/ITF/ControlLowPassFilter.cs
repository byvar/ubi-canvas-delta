namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class ControlLowPassFilter : SoundModifier {
		public ProceduralInputData input;
		public float frequency;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			input = s.SerializeObject<ProceduralInputData>(input, name: "input");
			frequency = s.Serialize<float>(frequency, name: "frequency");
		}
		public override uint? ClassCRC => 0xA28379D8;
	}
}

