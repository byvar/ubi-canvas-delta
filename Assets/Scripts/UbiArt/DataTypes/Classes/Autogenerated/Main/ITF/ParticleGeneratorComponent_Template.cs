namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class ParticleGeneratorComponent_Template : GraphicComponent_Template {
		public ITF_ParticleGenerator_Template ParticleGeneratorParams;
		public float startTime = 1f;
		public float stopTime = 1f;
		public Path texture;
		public GFXMaterialSerializable material;
		public bool beginStart = true;
		public CListO<InputDesc> inputs;
		public ProceduralInputData frequencyInput;
		public ProceduralInputData emitCountInput;
		public ProceduralInputData maxParticlesInput;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				ParticleGeneratorParams = s.SerializeObject<ITF_ParticleGenerator_Template>(ParticleGeneratorParams, name: "ParticleGeneratorParams");
				startTime = s.Serialize<float>(startTime, name: "startTime");
				stopTime = s.Serialize<float>(stopTime, name: "stopTime");
				texture = s.SerializeObject<Path>(texture, name: "texture");
				beginStart = s.Serialize<bool>(beginStart, name: "beginStart");
				inputs = s.SerializeObject<CListO<InputDesc>>(inputs, name: "inputs");
				frequencyInput = s.SerializeObject<ProceduralInputData>(frequencyInput, name: "frequencyInput");
				emitCountInput = s.SerializeObject<ProceduralInputData>(emitCountInput, name: "emitCountInput");
			} else if (s.Settings.Game == Game.COL) {
				ParticleGeneratorParams = s.SerializeObject<ITF_ParticleGenerator_Template>(ParticleGeneratorParams, name: "ParticleGeneratorParams");
				startTime = s.Serialize<float>(startTime, name: "startTime");
				stopTime = s.Serialize<float>(stopTime, name: "stopTime");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				beginStart = s.Serialize<bool>(beginStart, name: "beginStart", options: CSerializerObject.Options.BoolAsByte);
				frequencyInput = s.SerializeObject<ProceduralInputData>(frequencyInput, name: "frequencyInput");
				emitCountInput = s.SerializeObject<ProceduralInputData>(emitCountInput, name: "emitCountInput");
				maxParticlesInput = s.SerializeObject<ProceduralInputData>(maxParticlesInput, name: "maxParticlesInput");
			} else {
				ParticleGeneratorParams = s.SerializeObject<ITF_ParticleGenerator_Template>(ParticleGeneratorParams, name: "ParticleGeneratorParams");
				startTime = s.Serialize<float>(startTime, name: "startTime");
				stopTime = s.Serialize<float>(stopTime, name: "stopTime");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				beginStart = s.Serialize<bool>(beginStart, name: "beginStart");
				inputs = s.SerializeObject<CListO<InputDesc>>(inputs, name: "inputs");
				frequencyInput = s.SerializeObject<ProceduralInputData>(frequencyInput, name: "frequencyInput");
				emitCountInput = s.SerializeObject<ProceduralInputData>(emitCountInput, name: "emitCountInput");
				maxParticlesInput = s.SerializeObject<ProceduralInputData>(maxParticlesInput, name: "maxParticlesInput");
			}
		}
		public override uint? ClassCRC => 0xEF03E2F5;
	}
}

