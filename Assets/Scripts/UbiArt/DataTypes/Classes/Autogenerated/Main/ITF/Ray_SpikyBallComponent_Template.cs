namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_SpikyBallComponent_Template : ActorComponent_Template {
		public CListO<Ray_SpikyBall_Circle> circles;
		public CListO<Ray_SpikyBall_Transition> transitions;
		public Path texturePath;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			circles = s.SerializeObject<CListO<Ray_SpikyBall_Circle>>(circles, name: "circles");
			transitions = s.SerializeObject<CListO<Ray_SpikyBall_Transition>>(transitions, name: "transitions");
			texturePath = s.SerializeObject<Path>(texturePath, name: "texturePath");
		}
		public override uint? ClassCRC => 0x5600BB01;

		public class Ray_SpikyBall_Transition : CSerializable {
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
			public override uint? ClassCRC => 0x5DAA0E44;
		}

		public class Ray_SpikyBall_Circle : CSerializable {
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
			public override uint? ClassCRC => 0x92BBA8A1;
		}
	}
}

