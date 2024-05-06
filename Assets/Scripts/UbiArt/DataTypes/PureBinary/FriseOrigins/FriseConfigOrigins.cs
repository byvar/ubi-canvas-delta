using System.Linq;
using UbiArt.ITF;
using static UbiArt.ITF.Frise;

namespace UbiArt.FriseOrigins {
	// See: ITF::FriseConfig::serialize
	// fcg.ckd file
	public class FriseConfigOrigins : CSerializable {
		public uint version;
		public CListP<int> textureConfigIndexBySlope;
		public CListP<int> textureConfigIndexByZone;
		public CListO<FriseTextureConfig> textureConfigs;
		public int idTexFill;
		public Path gameMaterial;
		public Path gameMaterialExtremityStart;
		public Path gameMaterialExtremityStop;
		public StringID regionId;
		public bool flipCollision;
		public float unkFloat = 0.075f; // TODO
		public uint methodeCollision;
		public StringID regionId2;
		public string configName;
		public StringID configId;
		public int useFriseScale;
		public float flt1 = -2f; // TODO
		public int int2; // TODO
		public float visualOffset = 0.5f;
		public float forcedZ;
		public int isZForced;
		public float flexibility = 0.75f;
		public float wallAngle = 0.7853982f;
		public float cornerFactor = 1.5f;
		public float height = 2f;
		public float width = 2f;


		public uint methode = 1;
		public float snapCoeff = 0.1f;
		public float snapCoeffUv = 0.1f;
		public float density = 1f;
		public float scale = 1f;
		public bool smoothVisual = true;
		public bool smoothFill;
		public bool smoothCollision;
		public float smoothFactorVisual = 4f;
		public float smoothFactorFill = 2f;
		public float smoothFactorCollision = 2f;
		public uint uint0; // TODO
		public CollisionFrieze collision;

		public float fillOffset = 0.5f;
		public Angle fillAngle;
		public Vec2d fillScale = new Vec2d(2,2);

		public ITF.VertexAnim VertexAnim;

		public ColorInteger selfIllumColor;
		public ITF.FluidConfig Fluid;
		public int idTexSwitch = -1;
		public float zExtrudeUp;
		public float zExtrudeDown;
		public bool isUVFlipX;
		public bool isUVFlipY;
		public bool isRatioFixed = true;
		public bool fiesta_renderZRelated;
		public StringID sid1; // TODO
		public CMap<StringID, float> map; // TODO
		public int updateType;


		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			version = s.Serialize<uint>(version, name: "version");
			textureConfigIndexBySlope = s.SerializeObject<CListP<int>>(textureConfigIndexBySlope, name: "textureConfigIndexBySlope");
			textureConfigIndexByZone = s.SerializeObject<CListP<int>>(textureConfigIndexByZone, name: "textureConfigIndexByZone");
			textureConfigs = s.SerializeObject<CListO<FriseTextureConfig>>(textureConfigs, name: "textureConfigs");
			idTexFill = s.Serialize<int>(idTexFill, name: "idTexFill");
			gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
			gameMaterialExtremityStart = s.SerializeObject<Path>(gameMaterialExtremityStart, name: "gameMaterialExtremityStart");
			gameMaterialExtremityStop = s.SerializeObject<Path>(gameMaterialExtremityStop, name: "gameMaterialExtremityStop");
			regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
			flipCollision = s.Serialize<bool>(flipCollision, name: "flipCollision");
			unkFloat = s.Serialize<float>(unkFloat, name: "unkFloat");
			methodeCollision = s.Serialize<uint>(methodeCollision, name: "methodeCollision");
			regionId2 = s.SerializeObject<StringID>(regionId2, name: "regionId");
			configName = s.Serialize<string>(configName, name: "configName");
			configId = s.SerializeObject<StringID>(configId, name: "configId");

			useFriseScale = s.Serialize<int>(useFriseScale, name: "useFriseScale");
			flt1 = s.Serialize<float>(flt1, name: "flt1");
			int2 = s.Serialize<int>(int2, name: "int2");
			visualOffset = s.Serialize<float>(visualOffset, name: "visualOffset");
			forcedZ = s.Serialize<float>(forcedZ, name: "forcedZ");
			isZForced = s.Serialize<int>(isZForced, name: "isZForced");
			flexibility = s.Serialize<float>(flexibility, name: "flexibility");
			wallAngle = s.Serialize<float>(wallAngle, name: "wallAngle");
			cornerFactor = s.Serialize<float>(cornerFactor, name: "cornerFactor");
			height = s.Serialize<float>(height, name: "height");
			width = s.Serialize<float>(width, name: "witdh");

			methode = s.Serialize<uint>(methode, name: "methode");
			snapCoeff = s.Serialize<float>(snapCoeff, name: "snapCoeff");
			snapCoeffUv = s.Serialize<float>(snapCoeffUv, name: "snapCoeffUv");
			density = s.Serialize<float>(density, name: "density");
			scale = s.Serialize<float>(scale, name: "scale");
			smoothVisual = s.Serialize<bool>(smoothVisual, name: "smoothVisual");
			smoothFill = s.Serialize<bool>(smoothFill, name: "smoothFill");
			smoothCollision = s.Serialize<bool>(smoothCollision, name: "smoothCollision");
			smoothFactorVisual = s.Serialize<float>(smoothFactorVisual, name: "smoothFactorVisual");
			smoothFactorFill = s.Serialize<float>(smoothFactorFill, name: "smoothFactorFill");
			smoothFactorCollision = s.Serialize<float>(smoothFactorCollision, name: "smoothFactorCollision");
			uint0 = s.Serialize<uint>(uint0, name: "uint0");
			collision = s.SerializeObject<CollisionFrieze>(collision, name: "collision");

			fillOffset = s.Serialize<float>(fillOffset, name: "fillOffset");
			fillAngle = s.SerializeObject<Angle>(fillAngle, name: "fillAngle");
			fillScale = s.SerializeObject<Vec2d>(fillScale, name: "fillScale");

			VertexAnim = s.SerializeObject<ITF.VertexAnim>(VertexAnim, name: "VertexAnim");

			selfIllumColor = s.SerializeObject<ColorInteger>(selfIllumColor, name: "selfIllumColor");

			Fluid = s.SerializeObject<FluidConfig>(Fluid, name: "Fluid");

			idTexSwitch = s.Serialize<int>(idTexSwitch, name: "idTexSwitch");
			zExtrudeUp = s.Serialize<float>(zExtrudeUp, name: "zExtrudeUp");
			zExtrudeDown = s.Serialize<float>(zExtrudeDown, name: "zExtrudeDown");
			isUVFlipX = s.Serialize<bool>(isUVFlipX, name: "isUVFlipX");
			isUVFlipY = s.Serialize<bool>(isUVFlipY, name: "isUVFlipY");
			isRatioFixed = s.Serialize<bool>(isRatioFixed, name: "isRatioFixed");
			if (s.Settings.Game == Game.RFR) {
				fiesta_renderZRelated = s.Serialize<bool>(fiesta_renderZRelated, name: "fiesta_renderZRelated");
			}
			sid1 = s.SerializeObject<StringID>(sid1, name: "sid1");
			map = s.SerializeObject<CMap<StringID, float>>(map, name: "map");
			updateType = s.Serialize<int>(updateType, name: "updateType");
		}

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			foreach (var tex in textureConfigs) {
				s.Context.Loader.LoadTexture(tex.TexturePath, _ => { });
			}
		}

		public FriseConfig GetLegendsFriseConfig() {
			var fcg = new FriseConfig() {
				cooked = true, // Validate & process data breaks collision otherwise. Why?
				wallAngle = wallAngle,
				density = density,
				collision = new ITF.CollisionFrieze() {
					build = collision.build,
					//extremity = collision.extremity.x == float.MaxValue ? Vec2d.Infinity : collision.extremity,
					//extremity2 = collision.extremity2.x == float.MaxValue ? Vec2d.Infinity : collision.extremity2,
					extremity = collision.extremity,
					extremity2 = collision.extremity2,
					isSmooth = smoothCollision,
					methode = methodeCollision,
					flip = flipCollision,
					smoothFactor = smoothFactorCollision,
					offset = collision.offset,
				},
				methode = methode,
				isRatioFixed = isRatioFixed,
				cornerFactor = cornerFactor,
				fill = new FillConfig() {
					angle = fillAngle,
					scale = fillScale,
					offset = fillOffset,
					isSmooth = smoothFill,
					smoothFactor = smoothFactorFill,
					texIndex = idTexFill,
					tex = idTexFill != -1 ? textureConfigs[idTexFill].friendly?.str : null,
					zExtrude = 0f,
				},
				flexibility = flexibility,
				Fluid = Fluid,
				width = width,
				height = height,
				scale = scale,
				snapCoeff = snapCoeff,
				snapCoeffUv = snapCoeffUv,
				selfIllumColor = selfIllumColor,
				//zForced = forcedZ,
				//useZForced = isZForced,
				isUvFlipX = isUVFlipX,
				isUvFlipY = isUVFlipY,
				zExtrudeDown = zExtrudeDown,
				zExtrudeUp = zExtrudeUp,
				idTexSwitch = idTexSwitch,
				VertexAnim = VertexAnim,
				smoothFactorVisual = smoothFactorVisual,
				smoothVisual = smoothVisual,
				visualOffset = visualOffset,
				regionId = regionId,
				gameMaterial = gameMaterial,
				gameMaterialExtremityStart = gameMaterialExtremityStart,
				gameMaterialExtremityStop = gameMaterialExtremityStop,
				textureConfigIndexBySlope = textureConfigIndexBySlope,
				textureConfigIndexByZone = textureConfigIndexByZone,
				textureConfigs = new(textureConfigs.Select(tc => new ITF.FriseTextureConfig() {
					alphaBorder = tc.alphaBorder,
					alphaUp = tc.alphaUp,
					color = tc.color,
					friendly = new StringID(tc.friendly?.str),
					scrollAngle = tc.scrollAngle,
					scrollUV = new Vec2d(-tc.scrollUV.x, tc.scrollUV.y),
					gameMaterial = tc.gameMaterial,
					fillingOffset = tc.fillOffset,
					material = new GFXMaterialSerializable() {
						textureSet = new GFXMaterialTexturePathSet() {
							diffuse = tc.TexturePath,
							normal = tc.NormalPath
						},
						shaderPath = !tc.TexturePath.IsNull ? tc.RO2_ShaderPath : null
						// TODO: or "world/common/matshader/default.msh" ?
					},
					collision = new CollisionTexture() {
						build = tc.collision1.build ? (uint)1 : 0,
						offset = tc.collision1.offset,
						cornerIn = tc.collision1.cornerIn,
						cornerOut = tc.collision1.cornerOut,
						cornerInCur = tc.collision1.cornerInCur,
						cornerOutCur = tc.collision1.cornerOutCur
					}
				}).ToList())
			};
			/*if (fcg.gameMaterial == null || fcg.gameMaterial.IsNull) {
				fcg.gameMaterial = fcg.textureConfigs.FirstOrDefault(tc => tc.gameMaterial != null && !tc.gameMaterial.IsNull)?.gameMaterial;
				//fcg.gameMaterial = textureConfigs.FirstOrDefault(tc => !string.IsNullOrEmpty(tc.gameMaterial.FullPath)).gameMaterial;
			}*/
			/*foreach (var tcg in fcg.textureConfigs) {
				if (tcg.gameMaterial == null || tcg.gameMaterial.IsNull) {
					tcg.gameMaterial = fcg.gameMaterial;
				}
			}*/
			return fcg;
		}

		public Frise Instantiate(Path templatePath) {
			var basename = templatePath?.GetFilenameWithoutExtension(removeCooked: true);
			Frise frz = new Frise() {
				POS2D = Vec2d.Zero,
				USERFRIENDLY = basename,
				LUA = templatePath,
				ConfigName = templatePath,
				configOrigins = this,
				LOCAL_POINTS = new CListO<PolyLineEdge>(),
				PointsList = new PolyPointList() {
					LocalPoints = new CListO<PolyLineEdge>()
				},
				meshBuildData = new Nullable<MeshBuildData>(new MeshBuildData()),
				collisionData = new Nullable<CollisionData>(new CollisionData()),
				meshStaticData = new Nullable<MeshStaticData>(new MeshStaticData()),
				meshAnimData = new Nullable<MeshAnimData>(new MeshAnimData()),
				meshFluidData = new Nullable<MeshFluidData>(new MeshFluidData()),
				meshOverlayData = new Nullable<MeshOverlayData>(new MeshOverlayData()),
			};
			frz.InitContext(UbiArtContext);
			return (Frise)frz.Clone("frz");
		}
	}
}
