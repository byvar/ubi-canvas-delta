using System;
using System.Linq;

namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class FriseConfig : Actor_Template {
		public Path gameMaterial;
		public Path gameMaterialExtremityStart;
		public Path gameMaterialExtremityStop;
		public float visualOffset = 0.5f;
		public StringID regionId;
		public bool useFriezeFlipToFlipUV = true;
		public float cornerFactor = 1.5f;
		public float height = 2f;
		public float width = 2f;
		public uint methode = 1;
		public float snapCoeff = 0.1f;
		public float snapCoeffUv = 0.6f;
		public float flexibility = 0.75f;
		public Angle wallAngle = 0.7853982f;
		public bool isUvFlipX;
		public bool isUvFlipY;
		public bool isRatioFixed = true;
		public float smoothFactorVisual = float.MaxValue;
		public float scale = 1f;
		public float density = 1f;
		public CollisionFrieze collision;
		public FillConfig fill;
		public float zExtrudeUp;
		public float zExtrudeDown;
		public float zExtrudeStop;
		public float zExtrudeStart;
		public float zExtrudeExtremityStart;
		public float zExtrudeExtremityStop;
		public float patchCoeffOffset = 1f;
		public float patchScale = 1f;
		public bool patchOriented;
		public float patchCornerFactorCollision = 1f;
		public Angle patchAngleMin;
		public Vec2d ExtremityScale = Vec2d.One;
		public ColorInteger selfIllumColor;
		public CListO<FriseTextureConfig> textureConfigs;
		public CMap<StringID, Path> gameMaterials;
		public VertexAnim VertexAnim;
		public FluidConfig Fluid;
		public Mesh3dConfig mesh3D;
		public bool invertMeshOrder;
		public float skewAngle = float.MaxValue;
		public bool isDigShape;
		public bool isLockedDigShape;
		public bool switchExtremityAuto;
		public float offsetExtremity;
		public StringID slope_180;
		public StringID slope_202;
		public StringID slope_225;
		public StringID slope_247;
		public StringID slope_270;
		public StringID slope_292;
		public StringID slope_315;
		public StringID slope_337;
		public StringID slope_0;
		public StringID slope_22;
		public StringID slope_45;
		public StringID slope_67;
		public StringID slope_90;
		public StringID slope_112;
		public StringID slope_135;
		public StringID slope_157;
		public int idTexSwitch = -1;
		public bool cooked;
		public CListP<int> textureConfigIndexBySlope = new CListP<int>(Enumerable.Repeat(-1, 17).ToList());
		public CListP<int> textureConfigIndexByZone = new CListP<int>(Enumerable.Repeat(-1, 4).ToList());
		public bool smoothVisual;
		public GFXPrimitiveParam PrimitiveParameters;
		public Frieze3DConfig frieze3D;

		public float Vita_00 { get; set; }

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RJR) {
				throw new Exception(s.CurrentPointer + " - FriseConfig is internal/purebinary in RO version, but you can figure it out with RFR. TODO!");
			} else if (s.Settings.Game == Game.RL) {
				gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
				gameMaterialExtremityStart = s.SerializeObject<Path>(gameMaterialExtremityStart, name: "gameMaterialExtremityStart");
				gameMaterialExtremityStop = s.SerializeObject<Path>(gameMaterialExtremityStop, name: "gameMaterialExtremityStop");
				visualOffset = s.Serialize<float>(visualOffset, name: "visualOffset");
				regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
				useFriezeFlipToFlipUV = s.Serialize<bool>(useFriezeFlipToFlipUV, name: "useFriezeFlipToFlipUV");
				cornerFactor = s.Serialize<float>(cornerFactor, name: "cornerFactor");
				height = s.Serialize<float>(height, name: "height");
				width = s.Serialize<float>(width, name: "width");
				methode = s.Serialize<uint>(methode, name: "methode");
				snapCoeff = s.Serialize<float>(snapCoeff, name: "snapCoeff");
				snapCoeffUv = s.Serialize<float>(snapCoeffUv, name: "snapCoeffUv");
				flexibility = s.Serialize<float>(flexibility, name: "flexibility");
				wallAngle = s.SerializeObject<Angle>(wallAngle, name: "wallAngle");
				isUvFlipX = s.Serialize<bool>(isUvFlipX, name: "isUvFlipX");
				isUvFlipY = s.Serialize<bool>(isUvFlipY, name: "isUvFlipY");
				isRatioFixed = s.Serialize<bool>(isRatioFixed, name: "isRatioFixed");
				smoothFactorVisual = s.Serialize<float>(smoothFactorVisual, name: "smoothFactorVisual");
				if (s.Settings.Platform == GamePlatform.Vita) {
					Vita_00 = s.Serialize<float>(Vita_00, name: nameof(Vita_00));
				}
				scale = s.Serialize<float>(scale, name: "scale");
				density = s.Serialize<float>(density, name: "density");
				collision = s.SerializeObject<CollisionFrieze>(collision, name: "collision");
				fill = s.SerializeObject<FillConfig>(fill, name: "fill");
				zExtrudeUp = s.Serialize<float>(zExtrudeUp, name: "zExtrudeUp");
				zExtrudeDown = s.Serialize<float>(zExtrudeDown, name: "zExtrudeDown");
				zExtrudeStop = s.Serialize<float>(zExtrudeStop, name: "zExtrudeStop");
				zExtrudeStart = s.Serialize<float>(zExtrudeStart, name: "zExtrudeStart");
				zExtrudeExtremityStart = s.Serialize<float>(zExtrudeExtremityStart, name: "zExtrudeExtremityStart");
				zExtrudeExtremityStop = s.Serialize<float>(zExtrudeExtremityStop, name: "zExtrudeExtremityStop");
				patchCoeffOffset = s.Serialize<float>(patchCoeffOffset, name: "patchCoeffOffset");
				patchScale = s.Serialize<float>(patchScale, name: "patchScale");
				patchOriented = s.Serialize<bool>(patchOriented, name: "patchOriented");
				patchCornerFactorCollision = s.Serialize<float>(patchCornerFactorCollision, name: "patchCornerFactorCollision");
				patchAngleMin = s.SerializeObject<Angle>(patchAngleMin, name: "patchAngleMin");
				ExtremityScale = s.SerializeObject<Vec2d>(ExtremityScale, name: "ExtremityScale");
				selfIllumColor = s.SerializeObject<ColorInteger>(selfIllumColor, name: "selfIllumColor");
				textureConfigs = s.SerializeObject<CListO<FriseTextureConfig>>(textureConfigs, name: "textureConfigs");
				VertexAnim = s.SerializeObject<VertexAnim>(VertexAnim, name: "VertexAnim");
				Fluid = s.SerializeObject<FluidConfig>(Fluid, name: "Fluid");
				skewAngle = s.Serialize<float>(skewAngle, name: "skewAngle");
				isDigShape = s.Serialize<bool>(isDigShape, name: "isDigShape");
				isLockedDigShape = s.Serialize<bool>(isLockedDigShape, name: "isLockedDigShape");
				switchExtremityAuto = s.Serialize<bool>(switchExtremityAuto, name: "switchExtremityAuto");
				if (s.Settings.Platform != GamePlatform.Vita) {
					offsetExtremity = s.Serialize<float>(offsetExtremity, name: "offsetExtremity");
				}
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					slope_180 = s.SerializeObject<StringID>(slope_180, name: "slope_180");
					slope_202 = s.SerializeObject<StringID>(slope_202, name: "slope_202");
					slope_225 = s.SerializeObject<StringID>(slope_225, name: "slope_225");
					slope_247 = s.SerializeObject<StringID>(slope_247, name: "slope_247");
					slope_270 = s.SerializeObject<StringID>(slope_270, name: "slope_270");
					slope_292 = s.SerializeObject<StringID>(slope_292, name: "slope_292");
					slope_315 = s.SerializeObject<StringID>(slope_315, name: "slope_315");
					slope_337 = s.SerializeObject<StringID>(slope_337, name: "slope_337");
					slope_0 = s.SerializeObject<StringID>(slope_0, name: "slope_0");
					slope_22 = s.SerializeObject<StringID>(slope_22, name: "slope_22");
					slope_45 = s.SerializeObject<StringID>(slope_45, name: "slope_45");
					slope_67 = s.SerializeObject<StringID>(slope_67, name: "slope_67");
					slope_90 = s.SerializeObject<StringID>(slope_90, name: "slope_90");
					slope_112 = s.SerializeObject<StringID>(slope_112, name: "slope_112");
					slope_135 = s.SerializeObject<StringID>(slope_135, name: "slope_135");
					slope_157 = s.SerializeObject<StringID>(slope_157, name: "slope_157");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					idTexSwitch = s.Serialize<int>(idTexSwitch, name: "idTexSwitch");
					cooked = s.Serialize<bool>(cooked, name: "cooked");
					textureConfigIndexBySlope = s.SerializeObject<CListP<int>>(textureConfigIndexBySlope, name: "textureConfigIndexBySlope");
					textureConfigIndexByZone = s.SerializeObject<CListP<int>>(textureConfigIndexByZone, name: "textureConfigIndexByZone");
					smoothVisual = s.Serialize<bool>(smoothVisual, name: "smoothVisual");
				}
				PrimitiveParameters = s.SerializeObject<GFXPrimitiveParam>(PrimitiveParameters, name: "PrimitiveParameters");
			} else if (s.Settings.Game == Game.COL) {
				gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
				gameMaterialExtremityStart = s.SerializeObject<Path>(gameMaterialExtremityStart, name: "gameMaterialExtremityStart");
				gameMaterialExtremityStop = s.SerializeObject<Path>(gameMaterialExtremityStop, name: "gameMaterialExtremityStop");
				visualOffset = s.Serialize<float>(visualOffset, name: "visualOffset");
				regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
				useFriezeFlipToFlipUV = s.Serialize<bool>(useFriezeFlipToFlipUV, name: "useFriezeFlipToFlipUV", options: CSerializerObject.Options.BoolAsByte);
				cornerFactor = s.Serialize<float>(cornerFactor, name: "cornerFactor");
				height = s.Serialize<float>(height, name: "height");
				width = s.Serialize<float>(width, name: "width");
				methode = s.Serialize<uint>(methode, name: "methode");
				snapCoeff = s.Serialize<float>(snapCoeff, name: "snapCoeff");
				snapCoeffUv = s.Serialize<float>(snapCoeffUv, name: "snapCoeffUv");
				flexibility = s.Serialize<float>(flexibility, name: "flexibility");
				wallAngle = s.SerializeObject<Angle>(wallAngle, name: "wallAngle");
				isUvFlipX = s.Serialize<bool>(isUvFlipX, name: "isUvFlipX", options: CSerializerObject.Options.BoolAsByte);
				isUvFlipY = s.Serialize<bool>(isUvFlipY, name: "isUvFlipY", options: CSerializerObject.Options.BoolAsByte);
				isRatioFixed = s.Serialize<bool>(isRatioFixed, name: "isRatioFixed", options: CSerializerObject.Options.BoolAsByte);
				smoothFactorVisual = s.Serialize<float>(smoothFactorVisual, name: "smoothFactorVisual");
				scale = s.Serialize<float>(scale, name: "scale");
				density = s.Serialize<float>(density, name: "density");
				collision = s.SerializeObject<CollisionFrieze>(collision, name: "collision");
				fill = s.SerializeObject<FillConfig>(fill, name: "fill");
				zExtrudeUp = s.Serialize<float>(zExtrudeUp, name: "zExtrudeUp");
				zExtrudeDown = s.Serialize<float>(zExtrudeDown, name: "zExtrudeDown");
				zExtrudeStop = s.Serialize<float>(zExtrudeStop, name: "zExtrudeStop");
				zExtrudeStart = s.Serialize<float>(zExtrudeStart, name: "zExtrudeStart");
				zExtrudeExtremityStart = s.Serialize<float>(zExtrudeExtremityStart, name: "zExtrudeExtremityStart");
				zExtrudeExtremityStop = s.Serialize<float>(zExtrudeExtremityStop, name: "zExtrudeExtremityStop");
				patchCoeffOffset = s.Serialize<float>(patchCoeffOffset, name: "patchCoeffOffset");
				patchScale = s.Serialize<float>(patchScale, name: "patchScale");
				patchOriented = s.Serialize<bool>(patchOriented, name: "patchOriented");
				patchCornerFactorCollision = s.Serialize<float>(patchCornerFactorCollision, name: "patchCornerFactorCollision");
				patchAngleMin = s.SerializeObject<Angle>(patchAngleMin, name: "patchAngleMin");
				ExtremityScale = s.SerializeObject<Vec2d>(ExtremityScale, name: "ExtremityScale");
				selfIllumColor = s.SerializeObject<ColorInteger>(selfIllumColor, name: "selfIllumColor");
				textureConfigs = s.SerializeObject<CListO<FriseTextureConfig>>(textureConfigs, name: "textureConfigs");
				VertexAnim = s.SerializeObject<VertexAnim>(VertexAnim, name: "VertexAnim");
				Fluid = s.SerializeObject<FluidConfig>(Fluid, name: "Fluid");
				skewAngle = s.Serialize<float>(skewAngle, name: "skewAngle");
				isDigShape = s.Serialize<bool>(isDigShape, name: "isDigShape", options: CSerializerObject.Options.BoolAsByte);
				isLockedDigShape = s.Serialize<bool>(isLockedDigShape, name: "isLockedDigShape", options: CSerializerObject.Options.BoolAsByte);
				switchExtremityAuto = s.Serialize<bool>(switchExtremityAuto, name: "switchExtremityAuto");
				offsetExtremity = s.Serialize<float>(offsetExtremity, name: "offsetExtremity");
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					slope_180 = s.SerializeObject<StringID>(slope_180, name: "slope_180");
					slope_202 = s.SerializeObject<StringID>(slope_202, name: "slope_202");
					slope_225 = s.SerializeObject<StringID>(slope_225, name: "slope_225");
					slope_247 = s.SerializeObject<StringID>(slope_247, name: "slope_247");
					slope_270 = s.SerializeObject<StringID>(slope_270, name: "slope_270");
					slope_292 = s.SerializeObject<StringID>(slope_292, name: "slope_292");
					slope_315 = s.SerializeObject<StringID>(slope_315, name: "slope_315");
					slope_337 = s.SerializeObject<StringID>(slope_337, name: "slope_337");
					slope_0 = s.SerializeObject<StringID>(slope_0, name: "slope_0");
					slope_22 = s.SerializeObject<StringID>(slope_22, name: "slope_22");
					slope_45 = s.SerializeObject<StringID>(slope_45, name: "slope_45");
					slope_67 = s.SerializeObject<StringID>(slope_67, name: "slope_67");
					slope_90 = s.SerializeObject<StringID>(slope_90, name: "slope_90");
					slope_112 = s.SerializeObject<StringID>(slope_112, name: "slope_112");
					slope_135 = s.SerializeObject<StringID>(slope_135, name: "slope_135");
					slope_157 = s.SerializeObject<StringID>(slope_157, name: "slope_157");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					idTexSwitch = s.Serialize<int>(idTexSwitch, name: "idTexSwitch");
					cooked = s.Serialize<bool>(cooked, name: "cooked", options: CSerializerObject.Options.BoolAsByte);
					textureConfigIndexBySlope = s.SerializeObject<CListP<int>>(textureConfigIndexBySlope, name: "textureConfigIndexBySlope");
					textureConfigIndexByZone = s.SerializeObject<CListP<int>>(textureConfigIndexByZone, name: "textureConfigIndexByZone");
					smoothVisual = s.Serialize<bool>(smoothVisual, name: "smoothVisual", options: CSerializerObject.Options.BoolAsByte);
				}
				PrimitiveParameters = s.SerializeObject<GFXPrimitiveParam>(PrimitiveParameters, name: "PrimitiveParameters");
			} else if (s.Settings.Game == Game.VH) {
				gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
				gameMaterialExtremityStart = s.SerializeObject<Path>(gameMaterialExtremityStart, name: "gameMaterialExtremityStart");
				gameMaterialExtremityStop = s.SerializeObject<Path>(gameMaterialExtremityStop, name: "gameMaterialExtremityStop");
				visualOffset = s.Serialize<float>(visualOffset, name: "visualOffset");
				regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
				useFriezeFlipToFlipUV = s.Serialize<bool>(useFriezeFlipToFlipUV, name: "useFriezeFlipToFlipUV");
				cornerFactor = s.Serialize<float>(cornerFactor, name: "cornerFactor");
				height = s.Serialize<float>(height, name: "height");
				width = s.Serialize<float>(width, name: "width");
				methode = s.Serialize<uint>(methode, name: "methode");
				snapCoeff = s.Serialize<float>(snapCoeff, name: "snapCoeff");
				snapCoeffUv = s.Serialize<float>(snapCoeffUv, name: "snapCoeffUv");
				flexibility = s.Serialize<float>(flexibility, name: "flexibility");
				wallAngle = s.SerializeObject<Angle>(wallAngle, name: "wallAngle");
				isUvFlipX = s.Serialize<bool>(isUvFlipX, name: "isUvFlipX");
				isUvFlipY = s.Serialize<bool>(isUvFlipY, name: "isUvFlipY");
				isRatioFixed = s.Serialize<bool>(isRatioFixed, name: "isRatioFixed");
				smoothFactorVisual = s.Serialize<float>(smoothFactorVisual, name: "smoothFactorVisual");
				scale = s.Serialize<float>(scale, name: "scale");
				density = s.Serialize<float>(density, name: "density");
				collision = s.SerializeObject<CollisionFrieze>(collision, name: "collision");
				fill = s.SerializeObject<FillConfig>(fill, name: "fill");
				zExtrudeUp = s.Serialize<float>(zExtrudeUp, name: "zExtrudeUp");
				zExtrudeDown = s.Serialize<float>(zExtrudeDown, name: "zExtrudeDown");
				zExtrudeStop = s.Serialize<float>(zExtrudeStop, name: "zExtrudeStop");
				zExtrudeStart = s.Serialize<float>(zExtrudeStart, name: "zExtrudeStart");
				zExtrudeExtremityStart = s.Serialize<float>(zExtrudeExtremityStart, name: "zExtrudeExtremityStart");
				zExtrudeExtremityStop = s.Serialize<float>(zExtrudeExtremityStop, name: "zExtrudeExtremityStop");
				patchCoeffOffset = s.Serialize<float>(patchCoeffOffset, name: "patchCoeffOffset");
				patchScale = s.Serialize<float>(patchScale, name: "patchScale");
				patchOriented = s.Serialize<bool>(patchOriented, name: "patchOriented");
				patchCornerFactorCollision = s.Serialize<float>(patchCornerFactorCollision, name: "patchCornerFactorCollision");
				patchAngleMin = s.SerializeObject<Angle>(patchAngleMin, name: "patchAngleMin");
				ExtremityScale = s.SerializeObject<Vec2d>(ExtremityScale, name: "ExtremityScale");
				selfIllumColor = s.SerializeObject<ColorInteger>(selfIllumColor, name: "selfIllumColor");
				textureConfigs = s.SerializeObject<CListO<FriseTextureConfig>>(textureConfigs, name: "textureConfigs");
				VertexAnim = s.SerializeObject<VertexAnim>(VertexAnim, name: "VertexAnim");
				Fluid = s.SerializeObject<FluidConfig>(Fluid, name: "Fluid");
				mesh3D = s.SerializeObject<Mesh3dConfig>(mesh3D, name: "mesh3D");
				invertMeshOrder = s.Serialize<bool>(invertMeshOrder, name: "invertMeshOrder");
				skewAngle = s.Serialize<float>(skewAngle, name: "skewAngle");
				isDigShape = s.Serialize<bool>(isDigShape, name: "isDigShape");
				isLockedDigShape = s.Serialize<bool>(isLockedDigShape, name: "isLockedDigShape");
				switchExtremityAuto = s.Serialize<bool>(switchExtremityAuto, name: "switchExtremityAuto");
				offsetExtremity = s.Serialize<float>(offsetExtremity, name: "offsetExtremity");
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					slope_180 = s.SerializeObject<StringID>(slope_180, name: "slope_180");
					slope_202 = s.SerializeObject<StringID>(slope_202, name: "slope_202");
					slope_225 = s.SerializeObject<StringID>(slope_225, name: "slope_225");
					slope_247 = s.SerializeObject<StringID>(slope_247, name: "slope_247");
					slope_270 = s.SerializeObject<StringID>(slope_270, name: "slope_270");
					slope_292 = s.SerializeObject<StringID>(slope_292, name: "slope_292");
					slope_315 = s.SerializeObject<StringID>(slope_315, name: "slope_315");
					slope_337 = s.SerializeObject<StringID>(slope_337, name: "slope_337");
					slope_0 = s.SerializeObject<StringID>(slope_0, name: "slope_0");
					slope_22 = s.SerializeObject<StringID>(slope_22, name: "slope_22");
					slope_45 = s.SerializeObject<StringID>(slope_45, name: "slope_45");
					slope_67 = s.SerializeObject<StringID>(slope_67, name: "slope_67");
					slope_90 = s.SerializeObject<StringID>(slope_90, name: "slope_90");
					slope_112 = s.SerializeObject<StringID>(slope_112, name: "slope_112");
					slope_135 = s.SerializeObject<StringID>(slope_135, name: "slope_135");
					slope_157 = s.SerializeObject<StringID>(slope_157, name: "slope_157");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					idTexSwitch = s.Serialize<int>(idTexSwitch, name: "idTexSwitch");
					cooked = s.Serialize<bool>(cooked, name: "cooked");
					textureConfigIndexBySlope = s.SerializeObject<CListP<int>>(textureConfigIndexBySlope, name: "textureConfigIndexBySlope");
					textureConfigIndexByZone = s.SerializeObject<CListP<int>>(textureConfigIndexByZone, name: "textureConfigIndexByZone");
					smoothVisual = s.Serialize<bool>(smoothVisual, name: "smoothVisual");
				}
				PrimitiveParameters = s.SerializeObject<GFXPrimitiveParam>(PrimitiveParameters, name: "PrimitiveParameters");
				frieze3D = s.SerializeObject<Frieze3DConfig>(frieze3D, name: "frieze3D");
			} else {
				gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
				gameMaterialExtremityStart = s.SerializeObject<Path>(gameMaterialExtremityStart, name: "gameMaterialExtremityStart");
				gameMaterialExtremityStop = s.SerializeObject<Path>(gameMaterialExtremityStop, name: "gameMaterialExtremityStop");
				visualOffset = s.Serialize<float>(visualOffset, name: "visualOffset");
				regionId = s.SerializeObject<StringID>(regionId, name: "regionId");
				useFriezeFlipToFlipUV = s.Serialize<bool>(useFriezeFlipToFlipUV, name: "useFriezeFlipToFlipUV");
				cornerFactor = s.Serialize<float>(cornerFactor, name: "cornerFactor");
				height = s.Serialize<float>(height, name: "height");
				width = s.Serialize<float>(width, name: "width");
				methode = s.Serialize<uint>(methode, name: "methode");
				snapCoeff = s.Serialize<float>(snapCoeff, name: "snapCoeff");
				snapCoeffUv = s.Serialize<float>(snapCoeffUv, name: "snapCoeffUv");
				flexibility = s.Serialize<float>(flexibility, name: "flexibility");
				wallAngle = s.SerializeObject<Angle>(wallAngle, name: "wallAngle");
				isUvFlipX = s.Serialize<bool>(isUvFlipX, name: "isUvFlipX");
				isUvFlipY = s.Serialize<bool>(isUvFlipY, name: "isUvFlipY");
				isRatioFixed = s.Serialize<bool>(isRatioFixed, name: "isRatioFixed");
				smoothFactorVisual = s.Serialize<float>(smoothFactorVisual, name: "smoothFactorVisual");
				scale = s.Serialize<float>(scale, name: "scale");
				density = s.Serialize<float>(density, name: "density");
				collision = s.SerializeObject<CollisionFrieze>(collision, name: "collision");
				fill = s.SerializeObject<FillConfig>(fill, name: "fill");
				zExtrudeUp = s.Serialize<float>(zExtrudeUp, name: "zExtrudeUp");
				zExtrudeDown = s.Serialize<float>(zExtrudeDown, name: "zExtrudeDown");
				zExtrudeStop = s.Serialize<float>(zExtrudeStop, name: "zExtrudeStop");
				zExtrudeStart = s.Serialize<float>(zExtrudeStart, name: "zExtrudeStart");
				zExtrudeExtremityStart = s.Serialize<float>(zExtrudeExtremityStart, name: "zExtrudeExtremityStart");
				zExtrudeExtremityStop = s.Serialize<float>(zExtrudeExtremityStop, name: "zExtrudeExtremityStop");
				patchCoeffOffset = s.Serialize<float>(patchCoeffOffset, name: "patchCoeffOffset");
				patchScale = s.Serialize<float>(patchScale, name: "patchScale");
				patchOriented = s.Serialize<bool>(patchOriented, name: "patchOriented");
				patchCornerFactorCollision = s.Serialize<float>(patchCornerFactorCollision, name: "patchCornerFactorCollision");
				patchAngleMin = s.SerializeObject<Angle>(patchAngleMin, name: "patchAngleMin");
				ExtremityScale = s.SerializeObject<Vec2d>(ExtremityScale, name: "ExtremityScale");
				selfIllumColor = s.SerializeObject<ColorInteger>(selfIllumColor, name: "selfIllumColor");
				textureConfigs = s.SerializeObject<CListO<FriseTextureConfig>>(textureConfigs, name: "textureConfigs");
				gameMaterials = s.SerializeObject<CMap<StringID, Path>>(gameMaterials, name: "gameMaterials");
				VertexAnim = s.SerializeObject<VertexAnim>(VertexAnim, name: "VertexAnim");
				Fluid = s.SerializeObject<FluidConfig>(Fluid, name: "Fluid");
				mesh3D = s.SerializeObject<Mesh3dConfig>(mesh3D, name: "mesh3D");
				invertMeshOrder = s.Serialize<bool>(invertMeshOrder, name: "invertMeshOrder");
				skewAngle = s.Serialize<float>(skewAngle, name: "skewAngle");
				isDigShape = s.Serialize<bool>(isDigShape, name: "isDigShape");
				isLockedDigShape = s.Serialize<bool>(isLockedDigShape, name: "isLockedDigShape");
				switchExtremityAuto = s.Serialize<bool>(switchExtremityAuto, name: "switchExtremityAuto");
				offsetExtremity = s.Serialize<float>(offsetExtremity, name: "offsetExtremity");
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					slope_180 = s.SerializeObject<StringID>(slope_180, name: "slope_180");
					slope_202 = s.SerializeObject<StringID>(slope_202, name: "slope_202");
					slope_225 = s.SerializeObject<StringID>(slope_225, name: "slope_225");
					slope_247 = s.SerializeObject<StringID>(slope_247, name: "slope_247");
					slope_270 = s.SerializeObject<StringID>(slope_270, name: "slope_270");
					slope_292 = s.SerializeObject<StringID>(slope_292, name: "slope_292");
					slope_315 = s.SerializeObject<StringID>(slope_315, name: "slope_315");
					slope_337 = s.SerializeObject<StringID>(slope_337, name: "slope_337");
					slope_0 = s.SerializeObject<StringID>(slope_0, name: "slope_0");
					slope_22 = s.SerializeObject<StringID>(slope_22, name: "slope_22");
					slope_45 = s.SerializeObject<StringID>(slope_45, name: "slope_45");
					slope_67 = s.SerializeObject<StringID>(slope_67, name: "slope_67");
					slope_90 = s.SerializeObject<StringID>(slope_90, name: "slope_90");
					slope_112 = s.SerializeObject<StringID>(slope_112, name: "slope_112");
					slope_135 = s.SerializeObject<StringID>(slope_135, name: "slope_135");
					slope_157 = s.SerializeObject<StringID>(slope_157, name: "slope_157");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					idTexSwitch = s.Serialize<int>(idTexSwitch, name: "idTexSwitch");
					cooked = s.Serialize<bool>(cooked, name: "cooked");
					textureConfigIndexBySlope = s.SerializeObject<CListP<int>>(textureConfigIndexBySlope, name: "textureConfigIndexBySlope");
					textureConfigIndexByZone = s.SerializeObject<CListP<int>>(textureConfigIndexByZone, name: "textureConfigIndexByZone");
					smoothVisual = s.Serialize<bool>(smoothVisual, name: "smoothVisual");
				}
				PrimitiveParameters = s.SerializeObject<GFXPrimitiveParam>(PrimitiveParameters, name: "PrimitiveParameters");
				frieze3D = s.SerializeObject<Frieze3DConfig>(frieze3D, name: "frieze3D");
			}
		}
		public override uint? ClassCRC => 0xFEEFD98D;
	}
}

