namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class FluidConfig : CSerializable {
		public float Elasticity = 0.98f;
		public float Velocity = 0.04f;
		public float Viscosity = 0.05f;
		public float TargetHeight = 0.5f;
		public float Amplification = 0.01f;
		public float TargetMaxHeight = 1f;
		public float TargetMinHeight;
		public float TargetAddHeight = -0.005f;
		public float UnityWidth = 0.1f;
		public uint PolylineUnityMult = 1;
		public bool PolylineReaction = true;
		public float EnterMult = 10f;
		public float LeaveMult = 10f;
		public float MaxDstInfluence = 1f;
		public uint LevelsFront;
		public uint LevelsBack;
		public Vec3d LevelDelta;
		public Vec2d UVDelta;
		public uint PerpendicularBack;
		public float PerpendicularBackZ;
		public float PerpendicularBackScale = 1f;
		public float PerpendicularBackPos = 1f;
		public float PerpendicularBackPosZ;
		public uint PerpendicularFront;
		public float PerpendicularFrontZ;
		public float PerpendicularFrontScale = 1f;
		public float PerpendicularFrontPos = 1f;
		public float PerpendicularFrontPosZ;
		public float WeightMultiplier = 0.03f;
		public float dstInfluenceMultiplier = 1f;
		public float AbsorptionAtEdgeStart;
		public float AbsorptionAtEdgeEnd;
		public float AbsorptionAtEdge_Length;
		public float InfluenceLimit = float.MaxValue;
		public uint SideCount = 1;
		public uint LayerCount = 1;
		public float BlendFactor;
		public Path FxActor;
		public CListO<FluidFriseLayer> Layers;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RJR) {
				Elasticity = s.Serialize<float>(Elasticity, name: "Elasticity");
				Velocity = s.Serialize<float>(Velocity, name: "Velocity");
				Viscosity = s.Serialize<float>(Viscosity, name: "Viscosity");
				AbsorptionAtEdgeStart = s.Serialize<float>(AbsorptionAtEdgeStart, name: "AbsorptionAtEdgeStart");
				AbsorptionAtEdgeEnd = s.Serialize<float>(AbsorptionAtEdgeEnd, name: "AbsorptionAtEdgeEnd");
				AbsorptionAtEdge_Length = s.Serialize<float>(AbsorptionAtEdge_Length, name: "AbsorptionAtEdge_Length");
				TargetHeight = s.Serialize<float>(TargetHeight, name: "TargetHeight");
				Amplification = s.Serialize<float>(Amplification, name: "Amplification");
				TargetMaxHeight = s.Serialize<float>(TargetMaxHeight, name: "TargetMaxHeight");
				TargetMinHeight = s.Serialize<float>(TargetMinHeight, name: "TargetMinHeight");
				TargetAddHeight = s.Serialize<float>(TargetAddHeight, name: "TargetAddHeight");
				UnityWidth = s.Serialize<float>(UnityWidth, name: "UnityWidth");
				PolylineUnityMult = s.Serialize<uint>(PolylineUnityMult, name: "PolylineUnityMult");
				EnterMult = s.Serialize<float>(EnterMult, name: "EnterMult");
				LeaveMult = s.Serialize<float>(LeaveMult, name: "LeaveMult");
				MaxDstInfluence = s.Serialize<float>(MaxDstInfluence, name: "MaxDstInfluence");
				LevelsFront = s.Serialize<uint>(LevelsFront, name: "LevelsFront");
				LevelsBack = s.Serialize<uint>(LevelsBack, name: "LevelsBack");
				LevelDelta = s.SerializeObject<Vec3d>(LevelDelta, name: "LevelDelta");
				UVDelta = s.SerializeObject<Vec2d>(UVDelta, name: "UVDelta");
				PerpendicularBack = s.Serialize<uint>(PerpendicularBack, name: "PerpendicularBack");
				PerpendicularBackZ = s.Serialize<float>(PerpendicularBackZ, name: "PerpendicularBackZ");
				PerpendicularBackScale = s.Serialize<float>(PerpendicularBackScale, name: "PerpendicularBackScale");
				PerpendicularBackPos = s.Serialize<float>(PerpendicularBackPos, name: "PerpendicularBackPos");
				PerpendicularBackPosZ = s.Serialize<float>(PerpendicularBackPosZ, name: "PerpendicularBackPosZ");
				PerpendicularFront = s.Serialize<uint>(PerpendicularFront, name: "PerpendicularFront");
				PerpendicularFrontZ = s.Serialize<float>(PerpendicularFrontZ, name: "PerpendicularFrontZ");
				PerpendicularFrontScale = s.Serialize<float>(PerpendicularFrontScale, name: "PerpendicularFrontScale");
				PerpendicularFrontPos = s.Serialize<float>(PerpendicularFrontPos, name: "PerpendicularFrontPos");
				PerpendicularFrontPosZ = s.Serialize<float>(PerpendicularFrontPosZ, name: "PerpendicularFrontPosZ");
				WeightMultiplier = s.Serialize<float>(WeightMultiplier, name: "WeightMultiplier");
				dstInfluenceMultiplier = s.Serialize<float>(dstInfluenceMultiplier, name: "dstInfluenceMultiplier");
				InfluenceLimit = s.Serialize<float>(InfluenceLimit, name: "InfluenceLimit");
				LayerCount = s.Serialize<uint>(LayerCount, name: "LayerCount");
				Layers = s.SerializeObject<CListO<FluidFriseLayer>>(Layers, name: "Layers");
				SideCount = s.Serialize<uint>(SideCount, name: "SideCount");
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				Elasticity = s.Serialize<float>(Elasticity, name: "Elasticity");
				Velocity = s.Serialize<float>(Velocity, name: "Velocity");
				Viscosity = s.Serialize<float>(Viscosity, name: "Viscosity");
				TargetHeight = s.Serialize<float>(TargetHeight, name: "TargetHeight");
				Amplification = s.Serialize<float>(Amplification, name: "Amplification");
				TargetMaxHeight = s.Serialize<float>(TargetMaxHeight, name: "TargetMaxHeight");
				TargetMinHeight = s.Serialize<float>(TargetMinHeight, name: "TargetMinHeight");
				TargetAddHeight = s.Serialize<float>(TargetAddHeight, name: "TargetAddHeight");
				UnityWidth = s.Serialize<float>(UnityWidth, name: "UnityWidth");
				PolylineUnityMult = s.Serialize<uint>(PolylineUnityMult, name: "PolylineUnityMult");
				EnterMult = s.Serialize<float>(EnterMult, name: "EnterMult");
				LeaveMult = s.Serialize<float>(LeaveMult, name: "LeaveMult");
				MaxDstInfluence = s.Serialize<float>(MaxDstInfluence, name: "MaxDstInfluence");
				LevelsFront = s.Serialize<uint>(LevelsFront, name: "LevelsFront");
				LevelsBack = s.Serialize<uint>(LevelsBack, name: "LevelsBack");
				LevelDelta = s.SerializeObject<Vec3d>(LevelDelta, name: "LevelDelta");
				UVDelta = s.SerializeObject<Vec2d>(UVDelta, name: "UVDelta");
				PerpendicularBack = s.Serialize<uint>(PerpendicularBack, name: "PerpendicularBack");
				PerpendicularBackZ = s.Serialize<float>(PerpendicularBackZ, name: "PerpendicularBackZ");
				PerpendicularBackScale = s.Serialize<float>(PerpendicularBackScale, name: "PerpendicularBackScale");
				PerpendicularBackPos = s.Serialize<float>(PerpendicularBackPos, name: "PerpendicularBackPos");
				PerpendicularBackPosZ = s.Serialize<float>(PerpendicularBackPosZ, name: "PerpendicularBackPosZ");
				PerpendicularFront = s.Serialize<uint>(PerpendicularFront, name: "PerpendicularFront");
				PerpendicularFrontZ = s.Serialize<float>(PerpendicularFrontZ, name: "PerpendicularFrontZ");
				PerpendicularFrontScale = s.Serialize<float>(PerpendicularFrontScale, name: "PerpendicularFrontScale");
				PerpendicularFrontPos = s.Serialize<float>(PerpendicularFrontPos, name: "PerpendicularFrontPos");
				PerpendicularFrontPosZ = s.Serialize<float>(PerpendicularFrontPosZ, name: "PerpendicularFrontPosZ");
				WeightMultiplier = s.Serialize<float>(WeightMultiplier, name: "WeightMultiplier");
				dstInfluenceMultiplier = s.Serialize<float>(dstInfluenceMultiplier, name: "dstInfluenceMultiplier");
				AbsorptionAtEdgeStart = s.Serialize<float>(AbsorptionAtEdgeStart, name: "AbsorptionAtEdgeStart");
				AbsorptionAtEdgeEnd = s.Serialize<float>(AbsorptionAtEdgeEnd, name: "AbsorptionAtEdgeEnd");
				AbsorptionAtEdge_Length = s.Serialize<float>(AbsorptionAtEdge_Length, name: "AbsorptionAtEdge_Length");
				InfluenceLimit = s.Serialize<float>(InfluenceLimit, name: "InfluenceLimit");
				SideCount = s.Serialize<uint>(SideCount, name: "SideCount");
				LayerCount = s.Serialize<uint>(LayerCount, name: "LayerCount");
				BlendFactor = s.Serialize<float>(BlendFactor, name: "BlendFactor");
				FxActor = s.SerializeObject<Path>(FxActor, name: "FxActor");
				Layers = s.SerializeObject<CListO<FluidFriseLayer>>(Layers, name: "Layers");
			} else {
				Elasticity = s.Serialize<float>(Elasticity, name: "Elasticity");
				Velocity = s.Serialize<float>(Velocity, name: "Velocity");
				Viscosity = s.Serialize<float>(Viscosity, name: "Viscosity");
				TargetHeight = s.Serialize<float>(TargetHeight, name: "TargetHeight");
				Amplification = s.Serialize<float>(Amplification, name: "Amplification");
				TargetMaxHeight = s.Serialize<float>(TargetMaxHeight, name: "TargetMaxHeight");
				TargetMinHeight = s.Serialize<float>(TargetMinHeight, name: "TargetMinHeight");
				TargetAddHeight = s.Serialize<float>(TargetAddHeight, name: "TargetAddHeight");
				UnityWidth = s.Serialize<float>(UnityWidth, name: "UnityWidth");
				PolylineUnityMult = s.Serialize<uint>(PolylineUnityMult, name: "PolylineUnityMult");
				PolylineReaction = s.Serialize<bool>(PolylineReaction, name: "PolylineReaction");
				EnterMult = s.Serialize<float>(EnterMult, name: "EnterMult");
				LeaveMult = s.Serialize<float>(LeaveMult, name: "LeaveMult");
				MaxDstInfluence = s.Serialize<float>(MaxDstInfluence, name: "MaxDstInfluence");
				LevelsFront = s.Serialize<uint>(LevelsFront, name: "LevelsFront");
				LevelsBack = s.Serialize<uint>(LevelsBack, name: "LevelsBack");
				LevelDelta = s.SerializeObject<Vec3d>(LevelDelta, name: "LevelDelta");
				UVDelta = s.SerializeObject<Vec2d>(UVDelta, name: "UVDelta");
				PerpendicularBack = s.Serialize<uint>(PerpendicularBack, name: "PerpendicularBack");
				PerpendicularBackZ = s.Serialize<float>(PerpendicularBackZ, name: "PerpendicularBackZ");
				PerpendicularBackScale = s.Serialize<float>(PerpendicularBackScale, name: "PerpendicularBackScale");
				PerpendicularBackPos = s.Serialize<float>(PerpendicularBackPos, name: "PerpendicularBackPos");
				PerpendicularBackPosZ = s.Serialize<float>(PerpendicularBackPosZ, name: "PerpendicularBackPosZ");
				PerpendicularFront = s.Serialize<uint>(PerpendicularFront, name: "PerpendicularFront");
				PerpendicularFrontZ = s.Serialize<float>(PerpendicularFrontZ, name: "PerpendicularFrontZ");
				PerpendicularFrontScale = s.Serialize<float>(PerpendicularFrontScale, name: "PerpendicularFrontScale");
				PerpendicularFrontPos = s.Serialize<float>(PerpendicularFrontPos, name: "PerpendicularFrontPos");
				PerpendicularFrontPosZ = s.Serialize<float>(PerpendicularFrontPosZ, name: "PerpendicularFrontPosZ");
				WeightMultiplier = s.Serialize<float>(WeightMultiplier, name: "WeightMultiplier");
				dstInfluenceMultiplier = s.Serialize<float>(dstInfluenceMultiplier, name: "dstInfluenceMultiplier");
				AbsorptionAtEdgeStart = s.Serialize<float>(AbsorptionAtEdgeStart, name: "AbsorptionAtEdgeStart");
				AbsorptionAtEdgeEnd = s.Serialize<float>(AbsorptionAtEdgeEnd, name: "AbsorptionAtEdgeEnd");
				AbsorptionAtEdge_Length = s.Serialize<float>(AbsorptionAtEdge_Length, name: "AbsorptionAtEdge_Length");
				InfluenceLimit = s.Serialize<float>(InfluenceLimit, name: "InfluenceLimit");
				SideCount = s.Serialize<uint>(SideCount, name: "SideCount");
				LayerCount = s.Serialize<uint>(LayerCount, name: "LayerCount");
				BlendFactor = s.Serialize<float>(BlendFactor, name: "BlendFactor");
				FxActor = s.SerializeObject<Path>(FxActor, name: "FxActor");
				Layers = s.SerializeObject<CListO<FluidFriseLayer>>(Layers, name: "Layers");
			}
		}
	}
}

