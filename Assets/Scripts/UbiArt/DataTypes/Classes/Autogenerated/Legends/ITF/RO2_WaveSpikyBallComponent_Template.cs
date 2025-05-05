namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_WaveSpikyBallComponent_Template : ActorComponent_Template {
		public CListO<RO2_WaveSpikyBall_Circle> circles;
		public CListO<RO2_WaveSpikyBall_Transition> transitions;
		public Path texturePath;
		public GFXMaterialSerializable material;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			circles = s.SerializeObject<CListO<RO2_WaveSpikyBall_Circle>>(circles, name: "circles");
			transitions = s.SerializeObject<CListO<RO2_WaveSpikyBall_Transition>>(transitions, name: "transitions");
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				texturePath = s.SerializeObject<Path>(texturePath, name: "texturePath");
			}
			material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
		}
		public override uint? ClassCRC => 0x31D38AFF;

		public class RO2_WaveSpikyBall_Transition : CSerializable {
			public uint firstIndex;
			public uint secondIndex = 1;
			public float duration = 0.1f;
			public StringID fx;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				firstIndex = s.Serialize<uint>(firstIndex, name: "firstIndex");
				secondIndex = s.Serialize<uint>(secondIndex, name: "secondIndex");
				duration = s.Serialize<float>(duration, name: "duration");
				fx = s.SerializeObject<StringID>(fx, name: "fx");
			}
			public override uint? ClassCRC => 0x1CC29066;
		}

		public class RO2_WaveSpikyBall_Circle : CSerializable {
			public float radiusFactor;
			public float hurtRadiusFactor;
			public Color color;
			public bool hurts;
			public float spikeHeight;
			public StringID fx;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				radiusFactor = s.Serialize<float>(radiusFactor, name: "radiusFactor");
				hurtRadiusFactor = s.Serialize<float>(hurtRadiusFactor, name: "hurtRadiusFactor");
				color = s.SerializeObject<Color>(color, name: "color");
				hurts = s.Serialize<bool>(hurts, name: "hurts");
				spikeHeight = s.Serialize<float>(spikeHeight, name: "spikeHeight");
				fx = s.SerializeObject<StringID>(fx, name: "fx");
			}
			public override uint? ClassCRC => 0xDBF8F495;
		}
	}
}

