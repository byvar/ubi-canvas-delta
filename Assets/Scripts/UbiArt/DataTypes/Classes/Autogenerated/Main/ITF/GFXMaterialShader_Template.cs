namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class GFXMaterialShader_Template : TemplateObj {
		public uint flags;
		public bool renderRegular = true;
		public bool renderFrontLight;
		public bool renderBackLight;
		public int renderInReflection = -1;
		public bool useAlphaTest;
		public uint alphaRef = 0x80;
		public bool separateAlpha;
		public bool normalMapLighting;
		public bool textureBlend;
		public CListO<GFXMatAnimImpostor> animInTex;
		public GFX_MATERIAL_TYPE materialtype;
		public GFX_MATERIAL_TYPE_2 materialtype2;
		public GFX_MATERIAL_TYPE_3 materialtype3;
		public GFXMaterialLightingShader lightingType;
		public GFX_MaterialParams matParams;
		public GFX_BLENDMODE blendmode = GFX_BLENDMODE.ALPHA;
		public int renderToTexture;
		public bool renderGenerateBack;
		public COL_GFXMaterialShader_Layer_Template Layer1;
		public GFX_BLENDMODE BlendLayer2;
		public COL_GFXMaterialShader_Layer_Template Layer2;
		public GFX_BLENDMODE BlendLayer3;
		public COL_GFXMaterialShader_Layer_Template Layer3;
		public GFX_BLENDMODE BlendLayer4;
		public COL_GFXMaterialShader_Layer_Template Layer4;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				flags = s.Serialize<uint>(flags, name: "flags");
				renderRegular = s.Serialize<bool>(renderRegular, name: "renderRegular");
				renderFrontLight = s.Serialize<bool>(renderFrontLight, name: "renderFrontLight");
				renderBackLight = s.Serialize<bool>(renderBackLight, name: "renderBackLight");
				useAlphaTest = s.Serialize<bool>(useAlphaTest, name: "useAlphaTest");
				alphaRef = s.Serialize<uint>(alphaRef, name: "alphaRef");
				separateAlpha = s.Serialize<bool>(separateAlpha, name: "separateAlpha");
				textureBlend = s.Serialize<bool>(textureBlend, name: "textureBlend");
				renderToTexture = s.Serialize<int>(renderToTexture, name: "renderToTexture");
				if (s.Settings.Platform != GamePlatform.Vita) {
					animInTex = s.SerializeObject<CListO<GFXMatAnimImpostor>>(animInTex, name: "animInTex");
				}
				materialtype2 = s.Serialize<GFX_MATERIAL_TYPE_2>(materialtype2, name: "materialtype");
				lightingType = s.Serialize<GFXMaterialLightingShader>(lightingType, name: "lightingType");
				matParams = s.SerializeObject<GFX_MaterialParams>(matParams, name: "matParams");
				blendmode = s.Serialize<GFX_BLENDMODE>(blendmode, name: "blendmode");
			} else if (s.Settings.Game == Game.COL) {
				flags = s.Serialize<uint>(flags, name: "flags");
				renderRegular = s.Serialize<bool>(renderRegular, name: "renderRegular", options: CSerializerObject.Options.BoolAsByte);
				renderGenerateBack = s.Serialize<bool>(renderGenerateBack, name: "renderGenerateBack", options: CSerializerObject.Options.BoolAsByte);
				renderFrontLight = s.Serialize<bool>(renderFrontLight, name: "renderFrontLight", options: CSerializerObject.Options.BoolAsByte);
				renderBackLight = s.Serialize<bool>(renderBackLight, name: "renderBackLight", options: CSerializerObject.Options.BoolAsByte);
				useAlphaTest = s.Serialize<bool>(useAlphaTest, name: "useAlphaTest", options: CSerializerObject.Options.BoolAsByte);
				alphaRef = s.Serialize<uint>(alphaRef, name: "alphaRef");
				separateAlpha = s.Serialize<bool>(separateAlpha, name: "separateAlpha", options: CSerializerObject.Options.BoolAsByte);
				textureBlend = s.Serialize<bool>(textureBlend, name: "textureBlend", options: CSerializerObject.Options.BoolAsByte);
				animInTex = s.SerializeObject<CListO<GFXMatAnimImpostor>>(animInTex, name: "animInTex"); 
				materialtype3 = s.Serialize<GFX_MATERIAL_TYPE_3>(materialtype3, name: "materialtype");
				lightingType = s.Serialize<GFXMaterialLightingShader>(lightingType, name: "lightingType");
				matParams = s.SerializeObject<GFX_MaterialParams>(matParams, name: "matParams");
				blendmode = s.Serialize<GFX_BLENDMODE>(blendmode, name: "blendmode");
				Layer1 = s.SerializeObject<COL_GFXMaterialShader_Layer_Template>(Layer1, name: "Layer1");
				BlendLayer2 = s.Serialize<GFX_BLENDMODE>(BlendLayer2, name: "BlendLayer2");
				Layer2 = s.SerializeObject<COL_GFXMaterialShader_Layer_Template>(Layer2, name: "Layer2");
				BlendLayer3 = s.Serialize<GFX_BLENDMODE>(BlendLayer3, name: "BlendLayer3");
				Layer3 = s.SerializeObject<COL_GFXMaterialShader_Layer_Template>(Layer3, name: "Layer3");
				BlendLayer4 = s.Serialize<GFX_BLENDMODE>(BlendLayer4, name: "BlendLayer4");
				Layer4 = s.SerializeObject<COL_GFXMaterialShader_Layer_Template>(Layer4, name: "Layer4");
			} else {
				flags = s.Serialize<uint>(flags, name: "flags");
				renderRegular = s.Serialize<bool>(renderRegular, name: "renderRegular");
				renderFrontLight = s.Serialize<bool>(renderFrontLight, name: "renderFrontLight");
				renderBackLight = s.Serialize<bool>(renderBackLight, name: "renderBackLight");
				renderInReflection = s.Serialize<int>(renderInReflection, name: "renderInReflection");
				useAlphaTest = s.Serialize<bool>(useAlphaTest, name: "useAlphaTest");
				alphaRef = s.Serialize<uint>(alphaRef, name: "alphaRef");
				separateAlpha = s.Serialize<bool>(separateAlpha, name: "separateAlpha");
				normalMapLighting = s.Serialize<bool>(normalMapLighting, name: "normalMapLighting");
				textureBlend = s.Serialize<bool>(textureBlend, name: "textureBlend");
				animInTex = s.SerializeObject<CListO<GFXMatAnimImpostor>>(animInTex, name: "animInTex");
				materialtype = s.Serialize<GFX_MATERIAL_TYPE>(materialtype, name: "materialtype");
				lightingType = s.Serialize<GFXMaterialLightingShader>(lightingType, name: "lightingType");
				matParams = s.SerializeObject<GFX_MaterialParams>(matParams, name: "matParams");
				blendmode = s.Serialize<GFX_BLENDMODE>(blendmode, name: "blendmode");
			}
		}
		public enum GFX_MATERIAL_TYPE {
			[Serialize("GFX_MAT_DEFAULT"      )] DEFAULT = 0,
			[Serialize("GFX_MAT_REFRACTION"   )] REFRACTION = 1,
			[Serialize("GFX_MAT_PATCH"        )] PATCH = 2,
			[Serialize("GFX_MAT_FRIEZEANIM"   )] FRIEZEANIM = 3,
			[Serialize("GFX_MAT_GLOW"         )] GLOW = 4,
			[Serialize("GFX_MAT_ALPHAFADE"    )] ALPHAFADE = 5,
			[Serialize("GFX_MAT_FRIEZEOVERLAY")] FRIEZEOVERLAY = 6,
			[Serialize("GFX_MAT_REFLECTION"   )] REFLECTION = 7,
			GFX_MAT_FLUID_PARTICULE = 0x8,
			GFX_MAT_FLUID = 0x9,
			GFX_MAT_HIDDEN_MASK = 0xA,
			GFX_MAT_FLUID_STENCILPREPASS = 0xB,
			GFX_MAT_INVALID = -1,
		}
		public enum GFXMaterialLightingShader {
			[Serialize("GFX_MAT_SHADER_DEFAULT"      )] DEFAULT = 0,
			[Serialize("GFX_MAT_SHADER_LIGHT_LAYERED")] LIGHT_LAYERED = 1,
			GFX_MAT_SHADER_LIGHT_LAYERED_DEFAULT_BACK_FLAG = 0x2,
			GFX_MAT_SHADER_LIGHT_LAYERED_DEFAULT_BACK = 0x3,
			GFX_MAT_SHADER_LIGHT_LAYERED_DEFAULT_FRONT_FLAG = 0x4,
			GFX_MAT_SHADER_LIGHT_LAYERED_DEFAULT_FRONT = 0x5,
			GFX_MAT_SHADER_LIGHT_LAYERED_DEFAULT = 0x7,
			GFX_MAT_SHADER_LIGHT_USE_TANGENT_LIGHT = 0x8,
			GFX_MAT_SHADER_LIGHT_LAYERED_TANGENTLIGHT = 0x9,
			GFX_MAT_SHADER_LIGHT_LAYERED_LAST = 0x9,
			GFX_MAT_SHADER_INVALID = -1,
		}
		public enum GFX_BLENDMODE {
			[Serialize("GFX_BLEND_UNKNOWN"          )] UNKNOWN = 0,
			[Serialize("GFX_BLEND_COPY"             )] COPY = 1,
			[Serialize("GFX_BLEND_ALPHA"            )] ALPHA = 2,
			[Serialize("GFX_BLEND_ALPHAPREMULT"     )] ALPHAPREMULT = 3,
			[Serialize("GFX_BLEND_ALPHADEST"        )] ALPHADEST = 4,
			[Serialize("GFX_BLEND_ALPHADESTPREMULT" )] ALPHADESTPREMULT = 5,
			[Serialize("GFX_BLEND_ADD"              )] ADD = 6,
			[Serialize("GFX_BLEND_ADDALPHA"         )] ADDALPHA = 7,
			[Serialize("GFX_BLEND_SUBALPHA"         )] SUBALPHA = 8,
			[Serialize("GFX_BLEND_SUB"              )] SUB = 9,
			[Serialize("GFX_BLEND_MUL"              )] MUL = 10,
			[Serialize("GFX_BLEND_ALPHAMUL"         )] ALPHAMUL = 11,
			[Serialize("GFX_BLEND_IALPHAMUL"        )] IALPHAMUL = 12,
			[Serialize("GFX_BLEND_IALPHA"           )] IALPHA = 13,
			[Serialize("GFX_BLEND_IALPHAPREMULT"    )] IALPHAPREMULT = 14,
			[Serialize("GFX_BLEND_IALPHADEST"       )] IALPHADEST = 15,
			[Serialize("GFX_BLEND_IALPHADESTPREMULT")] IALPHADESTPREMULT = 16,
			[Serialize("GFX_BLEND_MUL2X"            )] MUL2X = 17,
			[Serialize("GFX_BLEND_ALPHATOCOLOR"     )] ALPHATOCOLOR = 18,
			[Serialize("GFX_BLEND_IALPHATOCOLOR"    )] IALPHATOCOLOR = 19,
			SETTOCOLOR = 20,
			[Serialize("GFX_BLEND_SCREEN"           )] SCREEN = 21,
			NUMBER = 22,
		}
		public enum GFX_MATERIAL_TYPE_2 {
			[Serialize("GFX_MAT_DEFAULT"      )] DEFAULT = 0,
			[Serialize("GFX_MAT_REFRACTION"   )] REFRACTION = 1,
			[Serialize("GFX_MAT_PATCH"        )] PATCH = 2,
			[Serialize("GFX_MAT_FRIEZEANIM"   )] FRIEZEANIM = 3,
			[Serialize("GFX_MAT_GLOW"         )] GLOW = 4,
			[Serialize("GFX_MAT_ALPHAFADE"    )] ALPHAFADE = 5,
			[Serialize("GFX_MAT_FRIEZEOVERLAY")] FRIEZEOVERLAY = 6,
			[Serialize("GFX_MAT_REFLECTION"   )] REFLECTION = 7,
			FLUID_PARTICULE = 0x8,
			FLUID = 0x9,
			HIDDEN_MASK = 0xA,
			FLUID_STENCILPREPASS = 0xB,
			[Serialize("Value_12")] Value_12 = 12,
			INVALID = -1,
		}
		public enum GFX_MATERIAL_TYPE_3 {
			[Serialize("GFX_MAT_DEFAULT"      )] DEFAULT = 0,
			[Serialize("GFX_MAT_REFRACTION"   )] REFRACTION = 1,
			[Serialize("GFX_MAT_PATCH"        )] PATCH = 2,
			[Serialize("GFX_MAT_FRIEZEANIM"   )] FRIEZEANIM = 3,
			[Serialize("GFX_MAT_GLOW"         )] GLOW = 4,
			[Serialize("GFX_MAT_ALPHAFADE"    )] ALPHAFADE = 5,
			[Serialize("GFX_MAT_FRIEZEOVERLAY")] FRIEZEOVERLAY = 6,
			[Serialize("GFX_MAT_REFLECTION"   )] REFLECTION = 7,
			FLUID_PARTICULE = 0x8,
			FLUID = 0x9,
			HIDDEN_MASK = 0xA,
			FLUID_STENCILPREPASS = 0xB,
			[Serialize("Value_12")] Value_12 = 12,
			[Serialize("Value_13")] Value_13 = 13,
			[Serialize("Value_14")] Value_14 = 14,
			[Serialize("Value_15")] Value_15 = 15,
			[Serialize("Value_16")] Value_16 = 16,
			INVALID = -1,
		}
		
		public override uint? ClassCRC => 0xE6A935E1;
	}
}

