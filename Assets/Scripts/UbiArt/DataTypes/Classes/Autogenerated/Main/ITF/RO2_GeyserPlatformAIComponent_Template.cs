namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_GeyserPlatformAIComponent_Template : GraphicComponent_Template {
		public StringID attachBone;
		public bool ignoreActorScale;
		public float length = 10f;
		public bool lockLength;
		public bool lockAngle = true;
		public bool lockWidth = true;
		public float growTargetSmoothFactor = 0.03f;
		public float growSmoothFactor = 0.03f;
		public float shrinkTargetSmoothFactor = 0.03f;
		public float shrinkSmoothFactor = 0.03f;
		public float lengthVariationAmplitude;
		public float lengthVariationFrequency = 1f;
		public float jumpThreshold = 0.5f;
		public bool triggerOnHit;
		public float triggerOnHitDelay;
		public float autoCloseDelay;
		public bool startOpen = true;
		public bool changeStateOnTrigger = true;
		public bool closeOnTap;
		public float closeOnTapDuration = 0.5f;
		public GFXMaterialSerializable material;
		public float patchStartOffset;
		public float patchLengthOffset;
		public float patchLengthMultiplier = 1f;
		public float patchWidthBottom = 1f;
		public float patchWidthTop = 2f;
		public float tileLength;
		public float tileWidth;
		public Vec2d uvScrollSpeed;
		public float patchFadeInLength = 2f;
		public float patchFadeOutLength = 2f;
		public float zOffset;
		public float tessellationLength = 1f;
		public PhysForceModifierPolygon_Template forceModifier;
		public Vec2d forceDirection = Vec2d.Right;
		public float forceLengthOffset;
		public float forceLengthMultiplier = 1f;
		public float forceWidthBottom = 1f;
		public float forceWidthTop = 2f;
		public float forceFadeOutLength;
		public bool overrideDisableForce;
		public bool usePlatform;
		public Path platformMaterial;
		public float platformWidth = 1f;
		public StringID platformBone;
		public StringID platformPolyline;
		public float platformLengthMultiplier = 1f;
		public float platformLengthOffset;
		public float platformHeightSink = 2f;
		public StringID fx;
		public bool fxUseTransform;
		public GeneratorType fxGeneratorType = GeneratorType.Box;
		public CListO<StringID> fxGenerators;
		public float fxLengthOffset;
		public float fxLengthMultiplier = 1f;
		public float fxWidth = 1f;
		public float fxLifetimeOffset;
		public float fxLifetimeMultiplier = 1f;
		public StringID fxCollision;
		public StringID fxOnClose;
		public float fxOnCloseThreshold = 0.1f;
		public StringID animOpened;
		public StringID animClosed;
		public StringID animOpenToClose;
		public bool useStim;
		public bool jumpToPlatform;
		public uint faction = uint.MaxValue;
		public bool registerToAIManager = true;
		public uint windTunnelLevel = uint.MaxValue;
		public CListO<RO2_GeyserPlatformAIComponent_Template.RegionData> regions;
		public bool isGeyserBehavior;
		public float widthReductionSpeed = 0.2f;
		public Angle minAngle;
		public Angle maxAngle;
		public int debugForce = 1;
		public int debugPatch;
		public int debugFx;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				attachBone = s.SerializeObject<StringID>(attachBone, name: "attachBone");
				ignoreActorScale = s.Serialize<bool>(ignoreActorScale, name: "ignoreActorScale");
				length = s.Serialize<float>(length, name: "length");
				lockLength = s.Serialize<bool>(lockLength, name: "lockLength", options: CSerializerObject.Options.BoolAsByte);
				lockAngle = s.Serialize<bool>(lockAngle, name: "lockAngle", options: CSerializerObject.Options.BoolAsByte);
				lockWidth = s.Serialize<bool>(lockWidth, name: "lockWidth", options: CSerializerObject.Options.BoolAsByte);
				growTargetSmoothFactor = s.Serialize<float>(growTargetSmoothFactor, name: "growTargetSmoothFactor");
				growSmoothFactor = s.Serialize<float>(growSmoothFactor, name: "growSmoothFactor");
				shrinkTargetSmoothFactor = s.Serialize<float>(shrinkTargetSmoothFactor, name: "shrinkTargetSmoothFactor");
				shrinkSmoothFactor = s.Serialize<float>(shrinkSmoothFactor, name: "shrinkSmoothFactor");
				lengthVariationAmplitude = s.Serialize<float>(lengthVariationAmplitude, name: "lengthVariationAmplitude");
				lengthVariationFrequency = s.Serialize<float>(lengthVariationFrequency, name: "lengthVariationFrequency");
				jumpThreshold = s.Serialize<float>(jumpThreshold, name: "jumpThreshold");
				triggerOnHit = s.Serialize<bool>(triggerOnHit, name: "triggerOnHit", options: CSerializerObject.Options.BoolAsByte);
				triggerOnHitDelay = s.Serialize<float>(triggerOnHitDelay, name: "triggerOnHitDelay");
				autoCloseDelay = s.Serialize<float>(autoCloseDelay, name: "autoCloseDelay");
				startOpen = s.Serialize<bool>(startOpen, name: "startOpen", options: CSerializerObject.Options.BoolAsByte);
				changeStateOnTrigger = s.Serialize<bool>(changeStateOnTrigger, name: "changeStateOnTrigger", options: CSerializerObject.Options.BoolAsByte);
				closeOnTap = s.Serialize<bool>(closeOnTap, name: "closeOnTap", options: CSerializerObject.Options.BoolAsByte);
				closeOnTapDuration = s.Serialize<float>(closeOnTapDuration, name: "closeOnTapDuration");
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				patchStartOffset = s.Serialize<float>(patchStartOffset, name: "patchStartOffset");
				patchLengthOffset = s.Serialize<float>(patchLengthOffset, name: "patchLengthOffset");
				patchLengthMultiplier = s.Serialize<float>(patchLengthMultiplier, name: "patchLengthMultiplier");
				patchWidthBottom = s.Serialize<float>(patchWidthBottom, name: "patchWidthBottom");
				patchWidthTop = s.Serialize<float>(patchWidthTop, name: "patchWidthTop");
				tileLength = s.Serialize<float>(tileLength, name: "tileLength");
				tileWidth = s.Serialize<float>(tileWidth, name: "tileWidth");
				uvScrollSpeed = s.SerializeObject<Vec2d>(uvScrollSpeed, name: "uvScrollSpeed");
				patchFadeInLength = s.Serialize<float>(patchFadeInLength, name: "patchFadeInLength");
				patchFadeOutLength = s.Serialize<float>(patchFadeOutLength, name: "patchFadeOutLength");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				tessellationLength = s.Serialize<float>(tessellationLength, name: "tessellationLength");
				forceModifier = s.SerializeObject<PhysForceModifierPolygon_Template>(forceModifier, name: "forceModifier");
				forceDirection = s.SerializeObject<Vec2d>(forceDirection, name: "forceDirection");
				forceLengthOffset = s.Serialize<float>(forceLengthOffset, name: "forceLengthOffset");
				forceLengthMultiplier = s.Serialize<float>(forceLengthMultiplier, name: "forceLengthMultiplier");
				forceWidthBottom = s.Serialize<float>(forceWidthBottom, name: "forceWidthBottom");
				forceWidthTop = s.Serialize<float>(forceWidthTop, name: "forceWidthTop");
				forceFadeOutLength = s.Serialize<float>(forceFadeOutLength, name: "forceFadeOutLength");
				overrideDisableForce = s.Serialize<bool>(overrideDisableForce, name: "overrideDisableForce");
				usePlatform = s.Serialize<bool>(usePlatform, name: "usePlatform", options: CSerializerObject.Options.BoolAsByte);
				platformMaterial = s.SerializeObject<Path>(platformMaterial, name: "platformMaterial");
				platformWidth = s.Serialize<float>(platformWidth, name: "platformWidth");
				platformBone = s.SerializeObject<StringID>(platformBone, name: "platformBone");
				platformPolyline = s.SerializeObject<StringID>(platformPolyline, name: "platformPolyline");
				platformLengthMultiplier = s.Serialize<float>(platformLengthMultiplier, name: "platformLengthMultiplier");
				platformLengthOffset = s.Serialize<float>(platformLengthOffset, name: "platformLengthOffset");
				platformHeightSink = s.Serialize<float>(platformHeightSink, name: "platformHeightSink");
				fx = s.SerializeObject<StringID>(fx, name: "fx");
				fxUseTransform = s.Serialize<bool>(fxUseTransform, name: "fxUseTransform", options: CSerializerObject.Options.BoolAsByte);
				fxGeneratorType = s.Serialize<GeneratorType>(fxGeneratorType, name: "fxGeneratorType");
				fxGenerators = s.SerializeObject<CListO<StringID>>(fxGenerators, name: "fxGenerators");
				fxLengthOffset = s.Serialize<float>(fxLengthOffset, name: "fxLengthOffset");
				fxLengthMultiplier = s.Serialize<float>(fxLengthMultiplier, name: "fxLengthMultiplier");
				fxWidth = s.Serialize<float>(fxWidth, name: "fxWidth");
				fxLifetimeOffset = s.Serialize<float>(fxLifetimeOffset, name: "fxLifetimeOffset");
				fxLifetimeMultiplier = s.Serialize<float>(fxLifetimeMultiplier, name: "fxLifetimeMultiplier");
				fxCollision = s.SerializeObject<StringID>(fxCollision, name: "fxCollision");
				fxOnClose = s.SerializeObject<StringID>(fxOnClose, name: "fxOnClose");
				fxOnCloseThreshold = s.Serialize<float>(fxOnCloseThreshold, name: "fxOnCloseThreshold");
				animOpened = s.SerializeObject<StringID>(animOpened, name: "animOpened");
				animClosed = s.SerializeObject<StringID>(animClosed, name: "animClosed");
				if (s.Settings.Platform != GamePlatform.Vita) {
					animOpenToClose = s.SerializeObject<StringID>(animOpenToClose, name: "animOpenToClose");
				}
				useStim = s.Serialize<bool>(useStim, name: "useStim", options: CSerializerObject.Options.BoolAsByte);
				jumpToPlatform = s.Serialize<bool>(jumpToPlatform, name: "jumpToPlatform", options: CSerializerObject.Options.BoolAsByte);
				faction = s.Serialize<uint>(faction, name: "faction");
				registerToAIManager = s.Serialize<bool>(registerToAIManager, name: "registerToAIManager", options: CSerializerObject.Options.BoolAsByte);
				windTunnelLevel = s.Serialize<uint>(windTunnelLevel, name: "windTunnelLevel");
				regions = s.SerializeObject<CListO<RO2_GeyserPlatformAIComponent_Template.RegionData>>(regions, name: "regions");
				isGeyserBehavior = s.Serialize<bool>(isGeyserBehavior, name: "isGeyserBehavior", options: CSerializerObject.Options.BoolAsByte);
				if (s.Settings.Platform != GamePlatform.Vita) {
					widthReductionSpeed = s.Serialize<float>(widthReductionSpeed, name: "widthReductionSpeed");
				}
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					minAngle = s.SerializeObject<Angle>(minAngle, name: "minAngle");
					maxAngle = s.SerializeObject<Angle>(maxAngle, name: "maxAngle");
					debugForce = s.Serialize<int>(debugForce, name: "debugForce");
					debugPatch = s.Serialize<int>(debugPatch, name: "debugPatch");
					debugFx = s.Serialize<int>(debugFx, name: "debugFx");
				}
			} else {
				attachBone = s.SerializeObject<StringID>(attachBone, name: "attachBone");
				ignoreActorScale = s.Serialize<bool>(ignoreActorScale, name: "ignoreActorScale");
				length = s.Serialize<float>(length, name: "length");
				lockLength = s.Serialize<bool>(lockLength, name: "lockLength");
				lockAngle = s.Serialize<bool>(lockAngle, name: "lockAngle");
				lockWidth = s.Serialize<bool>(lockWidth, name: "lockWidth");
				growTargetSmoothFactor = s.Serialize<float>(growTargetSmoothFactor, name: "growTargetSmoothFactor");
				growSmoothFactor = s.Serialize<float>(growSmoothFactor, name: "growSmoothFactor");
				shrinkTargetSmoothFactor = s.Serialize<float>(shrinkTargetSmoothFactor, name: "shrinkTargetSmoothFactor");
				shrinkSmoothFactor = s.Serialize<float>(shrinkSmoothFactor, name: "shrinkSmoothFactor");
				lengthVariationAmplitude = s.Serialize<float>(lengthVariationAmplitude, name: "lengthVariationAmplitude");
				lengthVariationFrequency = s.Serialize<float>(lengthVariationFrequency, name: "lengthVariationFrequency");
				jumpThreshold = s.Serialize<float>(jumpThreshold, name: "jumpThreshold");
				triggerOnHit = s.Serialize<bool>(triggerOnHit, name: "triggerOnHit");
				triggerOnHitDelay = s.Serialize<float>(triggerOnHitDelay, name: "triggerOnHitDelay");
				autoCloseDelay = s.Serialize<float>(autoCloseDelay, name: "autoCloseDelay");
				startOpen = s.Serialize<bool>(startOpen, name: "startOpen");
				changeStateOnTrigger = s.Serialize<bool>(changeStateOnTrigger, name: "changeStateOnTrigger");
				closeOnTap = s.Serialize<bool>(closeOnTap, name: "closeOnTap");
				closeOnTapDuration = s.Serialize<float>(closeOnTapDuration, name: "closeOnTapDuration");
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				patchStartOffset = s.Serialize<float>(patchStartOffset, name: "patchStartOffset");
				patchLengthOffset = s.Serialize<float>(patchLengthOffset, name: "patchLengthOffset");
				patchLengthMultiplier = s.Serialize<float>(patchLengthMultiplier, name: "patchLengthMultiplier");
				patchWidthBottom = s.Serialize<float>(patchWidthBottom, name: "patchWidthBottom");
				patchWidthTop = s.Serialize<float>(patchWidthTop, name: "patchWidthTop");
				tileLength = s.Serialize<float>(tileLength, name: "tileLength");
				tileWidth = s.Serialize<float>(tileWidth, name: "tileWidth");
				uvScrollSpeed = s.SerializeObject<Vec2d>(uvScrollSpeed, name: "uvScrollSpeed");
				patchFadeInLength = s.Serialize<float>(patchFadeInLength, name: "patchFadeInLength");
				patchFadeOutLength = s.Serialize<float>(patchFadeOutLength, name: "patchFadeOutLength");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				tessellationLength = s.Serialize<float>(tessellationLength, name: "tessellationLength");
				forceModifier = s.SerializeObject<PhysForceModifierPolygon_Template>(forceModifier, name: "forceModifier");
				forceDirection = s.SerializeObject<Vec2d>(forceDirection, name: "forceDirection");
				forceLengthOffset = s.Serialize<float>(forceLengthOffset, name: "forceLengthOffset");
				forceLengthMultiplier = s.Serialize<float>(forceLengthMultiplier, name: "forceLengthMultiplier");
				forceWidthBottom = s.Serialize<float>(forceWidthBottom, name: "forceWidthBottom");
				forceWidthTop = s.Serialize<float>(forceWidthTop, name: "forceWidthTop");
				forceFadeOutLength = s.Serialize<float>(forceFadeOutLength, name: "forceFadeOutLength");
				overrideDisableForce = s.Serialize<bool>(overrideDisableForce, name: "overrideDisableForce");
				usePlatform = s.Serialize<bool>(usePlatform, name: "usePlatform");
				platformMaterial = s.SerializeObject<Path>(platformMaterial, name: "platformMaterial");
				platformWidth = s.Serialize<float>(platformWidth, name: "platformWidth");
				platformBone = s.SerializeObject<StringID>(platformBone, name: "platformBone");
				platformPolyline = s.SerializeObject<StringID>(platformPolyline, name: "platformPolyline");
				platformLengthMultiplier = s.Serialize<float>(platformLengthMultiplier, name: "platformLengthMultiplier");
				platformLengthOffset = s.Serialize<float>(platformLengthOffset, name: "platformLengthOffset");
				platformHeightSink = s.Serialize<float>(platformHeightSink, name: "platformHeightSink");
				fx = s.SerializeObject<StringID>(fx, name: "fx");
				fxUseTransform = s.Serialize<bool>(fxUseTransform, name: "fxUseTransform");
				fxGeneratorType = s.Serialize<GeneratorType>(fxGeneratorType, name: "fxGeneratorType");
				fxGenerators = s.SerializeObject<CListO<StringID>>(fxGenerators, name: "fxGenerators");
				fxLengthOffset = s.Serialize<float>(fxLengthOffset, name: "fxLengthOffset");
				fxLengthMultiplier = s.Serialize<float>(fxLengthMultiplier, name: "fxLengthMultiplier");
				fxWidth = s.Serialize<float>(fxWidth, name: "fxWidth");
				fxLifetimeOffset = s.Serialize<float>(fxLifetimeOffset, name: "fxLifetimeOffset");
				fxLifetimeMultiplier = s.Serialize<float>(fxLifetimeMultiplier, name: "fxLifetimeMultiplier");
				fxCollision = s.SerializeObject<StringID>(fxCollision, name: "fxCollision");
				fxOnClose = s.SerializeObject<StringID>(fxOnClose, name: "fxOnClose");
				fxOnCloseThreshold = s.Serialize<float>(fxOnCloseThreshold, name: "fxOnCloseThreshold");
				animOpened = s.SerializeObject<StringID>(animOpened, name: "animOpened");
				animClosed = s.SerializeObject<StringID>(animClosed, name: "animClosed");
				animOpenToClose = s.SerializeObject<StringID>(animOpenToClose, name: "animOpenToClose");
				useStim = s.Serialize<bool>(useStim, name: "useStim");
				jumpToPlatform = s.Serialize<bool>(jumpToPlatform, name: "jumpToPlatform");
				faction = s.Serialize<uint>(faction, name: "faction");
				registerToAIManager = s.Serialize<bool>(registerToAIManager, name: "registerToAIManager");
				windTunnelLevel = s.Serialize<uint>(windTunnelLevel, name: "windTunnelLevel");
				regions = s.SerializeObject<CListO<RO2_GeyserPlatformAIComponent_Template.RegionData>>(regions, name: "regions");
				isGeyserBehavior = s.Serialize<bool>(isGeyserBehavior, name: "isGeyserBehavior");
				widthReductionSpeed = s.Serialize<float>(widthReductionSpeed, name: "widthReductionSpeed");
			}
		}
		[Games(GameFlags.VH | GameFlags.RA)]
		public partial class RegionData : CSerializable {
			public float widthOffset;
			public float heightOffset;
			public StringID id;
			public Path gameMaterial;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				widthOffset = s.Serialize<float>(widthOffset, name: "widthOffset");
				heightOffset = s.Serialize<float>(heightOffset, name: "heightOffset");
				id = s.SerializeObject<StringID>(id, name: "id");
				gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
			}
		}
		public enum GeneratorType {
			[Serialize("GeneratorType_Ballistic")] Ballistic = 0,
			[Serialize("GeneratorType_Box"      )] Box = 1,
		}
		public override uint? ClassCRC => 0xBA0DBA79;
	}
}

