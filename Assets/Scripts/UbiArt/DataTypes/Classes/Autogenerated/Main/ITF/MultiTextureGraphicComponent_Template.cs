namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MultiTextureGraphicComponent_Template : GraphicComponent_Template {
		public CListO<MultiTextureGraphicComponent_Template.MultiTextureEntry> textureList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			textureList = s.SerializeObject<CListO<MultiTextureGraphicComponent_Template.MultiTextureEntry>>(textureList, name: "textureList");
		}
		public override uint? ClassCRC => 0x0E87F074;


		[Games(GameFlags.COL)]
		public partial class MultiTextureEntry : CSerializable {
			public Path texture;
			public GFXMaterialSerializable material;
			public Color defaultColor = Color.White;
			public Angle angleX;
			public Angle angleY;
			public Angle angleZ;
			public float speedRotX;
			public float speedRotY;
			public float speedRotZ;
			public Vec2d size;
			public Vec2d posOffset;
			public float zOffset;
			public uint spriteIndex;
			public Enum_anchor anchor;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				defaultColor = s.SerializeObject<Color>(defaultColor, name: "defaultColor");
				angleX = s.SerializeObject<Angle>(angleX, name: "angleX");
				angleY = s.SerializeObject<Angle>(angleY, name: "angleY");
				angleZ = s.SerializeObject<Angle>(angleZ, name: "angleZ");
				speedRotX = s.Serialize<float>(speedRotX, name: "speedRotX");
				speedRotY = s.Serialize<float>(speedRotY, name: "speedRotY");
				speedRotZ = s.Serialize<float>(speedRotZ, name: "speedRotZ");
				size = s.SerializeObject<Vec2d>(size, name: "size");
				posOffset = s.SerializeObject<Vec2d>(posOffset, name: "posOffset");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				spriteIndex = s.Serialize<uint>(spriteIndex, name: "spriteIndex");
				size = s.SerializeObject<Vec2d>(size, name: "size");
				anchor = s.Serialize<Enum_anchor>(anchor, name: "anchor");
			}
			public enum Enum_anchor {
				[Serialize("Value_0")] Value_0 = 0,
				[Serialize("Value_1")] Value_1 = 1,
				[Serialize("Value_2")] Value_2 = 2,
				[Serialize("Value_3")] Value_3 = 3,
				[Serialize("Value_4")] Value_4 = 4,
				[Serialize("Value_5")] Value_5 = 5,
				[Serialize("Value_6")] Value_6 = 6,
				[Serialize("Value_7")] Value_7 = 7,
				[Serialize("Value_8")] Value_8 = 8,
			}
		}
	}
}

