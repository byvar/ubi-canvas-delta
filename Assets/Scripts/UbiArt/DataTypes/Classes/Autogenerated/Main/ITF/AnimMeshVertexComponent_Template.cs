namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class AnimMeshVertexComponent_Template : GraphicComponent_Template {
		public bool defaultUpdate;
		public bool draw2D;
		public Path amvPath;
		public Path texture;
		public GFXMaterialSerializable material;
		public bool useEditor = true;
		public bool useDataAnims = true;
		public bool useActorScale = true;
		public float stopDuration;
		public AnimResourcePackage animPackage;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				defaultUpdate = s.Serialize<bool>(defaultUpdate, name: "defaultUpdate");
				draw2D = s.Serialize<bool>(draw2D, name: "draw2D");
				amvPath = s.SerializeObject<Path>(amvPath, name: "amvPath");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				useEditor = s.Serialize<bool>(useEditor, name: "useEditor");
				useDataAnims = s.Serialize<bool>(useDataAnims, name: "useDataAnims");
				useActorScale = s.Serialize<bool>(useActorScale, name: "useActorScale");
				stopDuration = s.Serialize<float>(stopDuration, name: "stopDuration");
				if (s.Settings.Platform != GamePlatform.Vita) {
					animPackage = s.SerializeObject<AnimResourcePackage>(animPackage, name: "animPackage");
				}
			} else if(s.Settings.Game == Game.COL) {
				defaultUpdate = s.Serialize<bool>(defaultUpdate, name: "defaultUpdate", options: CSerializerObject.Options.BoolAsByte);
				draw2D = s.Serialize<bool>(draw2D, name: "draw2D", options: CSerializerObject.Options.BoolAsByte);
				amvPath = s.SerializeObject<Path>(amvPath, name: "amvPath");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				useEditor = s.Serialize<bool>(useEditor, name: "useEditor", options: CSerializerObject.Options.BoolAsByte);
				useDataAnims = s.Serialize<bool>(useDataAnims, name: "useDataAnims", options: CSerializerObject.Options.BoolAsByte);
				useActorScale = s.Serialize<bool>(useActorScale, name: "useActorScale", options: CSerializerObject.Options.BoolAsByte);
				stopDuration = s.Serialize<float>(stopDuration, name: "stopDuration");
				if (s.Settings.Platform != GamePlatform.Vita) {
					animPackage = s.SerializeObject<AnimResourcePackage>(animPackage, name: "animPackage");
				}
			} else {
				defaultUpdate = s.Serialize<bool>(defaultUpdate, name: "defaultUpdate");
				draw2D = s.Serialize<bool>(draw2D, name: "draw2D");
				amvPath = s.SerializeObject<Path>(amvPath, name: "amvPath");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				useEditor = s.Serialize<bool>(useEditor, name: "useEditor");
				useDataAnims = s.Serialize<bool>(useDataAnims, name: "useDataAnims");
				useActorScale = s.Serialize<bool>(useActorScale, name: "useActorScale");
				stopDuration = s.Serialize<float>(stopDuration, name: "stopDuration");
			}
		}
		public override uint? ClassCRC => 0x35E20242;
	}
}

