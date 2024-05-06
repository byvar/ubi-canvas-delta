namespace UbiArt.FriseOrigins {
	// See: ITF::FriseTextureConfig::serialize
	public class FriseTextureConfig : CSerializable {
		public CString path;
		public CString normal;
		public CString anim;
		public float flt3;
		public float flt4;
		public float flt5;
		public float flt6;
		public float flt7;
		public int int8;
		public int int9;
		public int int10;
		public ColorInteger color_2;
		public Vec2d vecA;
		public Vec2d vecB;
		public float flt8;
		public Path gameMaterial;
		public CString friendly;
		public ColorInteger color;
		public CollisionFrieze collision1;
		public CollisionFrieze collision2;
		public Vec2d scrollUV;
		public float scrollAngle;
		public byte alphaBorder;
		public byte alphaUp;

		// Below is all GFX_MATERIAL
		public GFX_BLEND blendmode = GFX_BLEND.ALPHA;
		public GFX_MAT materialtype;
		public float flt10;
		public float flt11;
		public float flt12;
		public float flt13;
		public int int0;
		public int int1;
		public int int2;
		// ^ GFX_MATERIAL

		public uint textureIsAnim;
		public float fillOffset;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			path = s.Serialize<CString>(path, name: "path");
			normal = s.Serialize<CString>(normal, name: "normal");
			anim = s.Serialize<CString>(anim, name: "anim");
			flt3 = s.Serialize<float>(flt3, name: "flt3");
			flt4 = s.Serialize<float>(flt4, name: "flt4");
			flt5 = s.Serialize<float>(flt5, name: "flt5");
			flt6 = s.Serialize<float>(flt6, name: "flt6");
			flt7 = s.Serialize<float>(flt7, name: "flt7");
			int8 = s.Serialize<int>(int8, name: "int8");
			int9 = s.Serialize<int>(int9, name: "int9");
			int10 = s.Serialize<int>(int10, name: "int10");
			color_2 = s.SerializeObject<ColorInteger>(color_2, name: "color_2");
			vecA = s.SerializeObject<Vec2d>(vecA, name: "vecA");
			vecB = s.SerializeObject<Vec2d>(vecB, name: "vecB");
			flt8 = s.Serialize<float>(flt8, name: "flt8");
			gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
			friendly = s.Serialize<CString>(friendly, name: "friendly");
			color = s.SerializeObject<ColorInteger>(color, name: "color");
			collision1 = s.SerializeObject<CollisionFrieze>(collision1, name: "collision1");
			collision2 = s.SerializeObject<CollisionFrieze>(collision2, name: "collision2");
			scrollUV = s.SerializeObject<Vec2d>(scrollUV, name: "scrollUV");
			scrollAngle = s.Serialize<float>(scrollAngle, name: "scrollAngle");
			alphaBorder = s.Serialize<byte>(alphaBorder, name: "alphaBorder");
			alphaUp = s.Serialize<byte>(alphaUp, name: "alphaUp");
			blendmode = s.Serialize<GFX_BLEND>(blendmode, name: "blendmode");
			materialtype = s.Serialize<GFX_MAT>(materialtype, name: "materialtype");
			flt10 = s.Serialize<float>(flt10, name: "flt10");
			flt11 = s.Serialize<float>(flt11, name: "flt11");
			flt12 = s.Serialize<float>(flt12, name: "flt12");
			flt13 = s.Serialize<float>(flt13, name: "flt13");
			int0 = s.Serialize<int>(int0, name: "int0");
			int1 = s.Serialize<int>(int1, name: "int1");
			int2 = s.Serialize<int>(int2, name: "int2");
			textureIsAnim = s.Serialize<uint>(textureIsAnim, name: "textureIsAnim");
			fillOffset = s.Serialize<float>(fillOffset, name: "fillOffset");
		}

		
		public enum GFX_MAT {
			[Serialize("GFX_MAT_DEFAULT"      )] DEFAULT = 0,
			[Serialize("GFX_MAT_REFRACTION"   )] REFRACTION = 1,
			[Serialize("GFX_MAT_PATCH"        )] PATCH = 2,
			[Serialize("GFX_MAT_FRIEZEANIM"   )] FRIEZEANIM = 3,
			[Serialize("GFX_MAT_GLOW"         )] GLOW = 4,
			[Serialize("GFX_MAT_ALPHAFADE"    )] ALPHAFADE = 5,
			[Serialize("GFX_MAT_FRIEZEOVERLAY")] FRIEZEOVERLAY = 6,
		}
		public enum GFX_BLEND {
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
		}



		public Path TexturePath => new Path(string.IsNullOrEmpty(path?.str) ? null : path.str);
		public Path NormalPath => new Path(string.IsNullOrEmpty(normal?.str) ? null : normal.str);

		public Path RO2_ShaderPath {
			get {
				var blend = blendmode;
				string shaderPath = blend switch {
					GFX_BLEND.ADDALPHA => "world/common/matshader/addalpha.msh",
					GFX_BLEND.ADD => "world/common/matshader/add.msh",
					GFX_BLEND.MUL => "world/common/matshader/mul.msh",
					GFX_BLEND.MUL2X => "world/common/matshader/mul2x.msh",
					_ => "world/common/matshader/regularbuffer/backlighted.msh" //"world/common/matshader/default.msh",
				};
				return new Path(shaderPath);
			}
		}
	}
}
