namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class PerlinNoise_Template : CSerializable {
		public float frequency = 1;
		public float persistence = 0.4f;
		public uint nbOctave = 4;
		public float amplitude = 1;
		public float timeMultiplier = 1;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			frequency = s.Serialize<float>(frequency, name: "frequency");
			persistence = s.Serialize<float>(persistence, name: "persistence");
			nbOctave = s.Serialize<uint>(nbOctave, name: "nbOctave");
			amplitude = s.Serialize<float>(amplitude, name: "amplitude");
			timeMultiplier = s.Serialize<float>(timeMultiplier, name: "timeMultiplier");
		}
	}
}

