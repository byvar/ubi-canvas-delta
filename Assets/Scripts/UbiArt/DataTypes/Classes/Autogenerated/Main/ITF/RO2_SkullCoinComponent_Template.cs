namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_SkullCoinComponent_Template : GraphicComponent_Template {
		public uint lumCount = 25;
		public float challengeDuration = 20;
		public float bezierUPerMeter = 1;
		public float bezierStartWidth = 0.2f;
		public float bezierEndWidth = 0.2f;
		public float tesselationLength = 0.3f;
		public uint blendMode = 7;
		public Path texture;
		public GFXMaterialSerializable material;
		public float backToNormalSpeed = 2;
		public float minPulsation = 1;
		public float maxPulsation = 2;
		public float minScaleAtStart = 2;
		public float maxScaleAtStart = 2;
		public float minScaleAtEnd = 0.5f;
		public float maxScaleAtEnd = 3;
		public float linkPulsation = 0.5f;
		public float linkAmplitude = 0.1f;
		public Vec2d playerFollowOffset;
		public float speedBlend = 1;
		public float speedMin;
		public float speedMax = 1;
		public float blendAtSpeedMin;
		public float blendAtSpeedMax = 1;
		public float backToNormalInertia = 0.1f;
		public float backToNormalBounciness = 0.5f;
		public float backToNormalInitSpeed = 10;
		public float maxDistanceBeforeExploding = 20;
		public float DRCMaxPickingDistance = 5;
		public float radius = 1;
		public float minLinkThicknessFactor = 0.5f;
		public float lenghtForMinThickness = 5;
		public Vec2d uvAnimTrans;
		public float startAlphaLen;
		public float endAlphaLen;
		public float linkAlphaMultiplier = 1;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			lumCount = s.Serialize<uint>(lumCount, name: "lumCount");
			challengeDuration = s.Serialize<float>(challengeDuration, name: "challengeDuration");
			bezierUPerMeter = s.Serialize<float>(bezierUPerMeter, name: "bezierUPerMeter");
			bezierStartWidth = s.Serialize<float>(bezierStartWidth, name: "bezierStartWidth");
			bezierEndWidth = s.Serialize<float>(bezierEndWidth, name: "bezierEndWidth");
			tesselationLength = s.Serialize<float>(tesselationLength, name: "tesselationLength");
			blendMode = s.Serialize<uint>(blendMode, name: "blendMode");
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				texture = s.SerializeObject<Path>(texture, name: "texture");
			}
			material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
			backToNormalSpeed = s.Serialize<float>(backToNormalSpeed, name: "backToNormalSpeed");
			minPulsation = s.Serialize<float>(minPulsation, name: "minPulsation");
			maxPulsation = s.Serialize<float>(maxPulsation, name: "maxPulsation");
			minScaleAtStart = s.Serialize<float>(minScaleAtStart, name: "minScaleAtStart");
			maxScaleAtStart = s.Serialize<float>(maxScaleAtStart, name: "maxScaleAtStart");
			minScaleAtEnd = s.Serialize<float>(minScaleAtEnd, name: "minScaleAtEnd");
			maxScaleAtEnd = s.Serialize<float>(maxScaleAtEnd, name: "maxScaleAtEnd");
			linkPulsation = s.Serialize<float>(linkPulsation, name: "linkPulsation");
			linkAmplitude = s.Serialize<float>(linkAmplitude, name: "linkAmplitude");
			playerFollowOffset = s.SerializeObject<Vec2d>(playerFollowOffset, name: "playerFollowOffset");
			speedBlend = s.Serialize<float>(speedBlend, name: "speedBlend");
			speedMin = s.Serialize<float>(speedMin, name: "speedMin");
			speedMax = s.Serialize<float>(speedMax, name: "speedMax");
			blendAtSpeedMin = s.Serialize<float>(blendAtSpeedMin, name: "blendAtSpeedMin");
			blendAtSpeedMax = s.Serialize<float>(blendAtSpeedMax, name: "blendAtSpeedMax");
			backToNormalInertia = s.Serialize<float>(backToNormalInertia, name: "backToNormalInertia");
			backToNormalBounciness = s.Serialize<float>(backToNormalBounciness, name: "backToNormalBounciness");
			backToNormalInitSpeed = s.Serialize<float>(backToNormalInitSpeed, name: "backToNormalInitSpeed");
			maxDistanceBeforeExploding = s.Serialize<float>(maxDistanceBeforeExploding, name: "maxDistanceBeforeExploding");
			DRCMaxPickingDistance = s.Serialize<float>(DRCMaxPickingDistance, name: "DRCMaxPickingDistance");
			radius = s.Serialize<float>(radius, name: "radius");
			minLinkThicknessFactor = s.Serialize<float>(minLinkThicknessFactor, name: "minLinkThicknessFactor");
			lenghtForMinThickness = s.Serialize<float>(lenghtForMinThickness, name: "lenghtForMinThickness");
			uvAnimTrans = s.SerializeObject<Vec2d>(uvAnimTrans, name: "uvAnimTrans");
			startAlphaLen = s.Serialize<float>(startAlphaLen, name: "startAlphaLen");
			endAlphaLen = s.Serialize<float>(endAlphaLen, name: "endAlphaLen");
			linkAlphaMultiplier = s.Serialize<float>(linkAlphaMultiplier, name: "linkAlphaMultiplier");
		}
		public override uint? ClassCRC => 0xB4C12CE0;
	}
}

