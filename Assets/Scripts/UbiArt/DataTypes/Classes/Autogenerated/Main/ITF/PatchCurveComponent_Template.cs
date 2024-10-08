namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PatchCurveComponent_Template : GraphicComponent_Template {
		public Path texture;
		public GFXMaterialSerializable material;
		public float widthStart = 1f;
		public float widthEnd = 1f;
		public float tileLength;
		public float uvScrollSpeed;
		public float zOffset;
		public float tessellationLength = 0.1f;
		public StringID parentBone = "SnapPoint";
		public StringID childBone = "SnapPoint";
		public float childOrientationInfluence = 0.5f;
		public bool attachToChild;
		public float width = 1f;
		public bool parentBoneEnd;
		public bool childBoneEnd;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				texture = s.SerializeObject<Path>(texture, name: "texture");
				width = s.Serialize<float>(width, name: "width");
				tileLength = s.Serialize<float>(tileLength, name: "tileLength");
				uvScrollSpeed = s.Serialize<float>(uvScrollSpeed, name: "uvScrollSpeed");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				tessellationLength = s.Serialize<float>(tessellationLength, name: "tessellationLength");
				parentBone = s.SerializeObject<StringID>(parentBone, name: "parentBone");
				childBone = s.SerializeObject<StringID>(childBone, name: "childBone");
				childOrientationInfluence = s.Serialize<float>(childOrientationInfluence, name: "childOrientationInfluence");
				attachToChild = s.Serialize<bool>(attachToChild, name: "attachToChild");
			} else if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				widthStart = s.Serialize<float>(widthStart, name: "widthStart");
				widthEnd = s.Serialize<float>(widthEnd, name: "widthEnd");
				tileLength = s.Serialize<float>(tileLength, name: "tileLength");
				uvScrollSpeed = s.Serialize<float>(uvScrollSpeed, name: "uvScrollSpeed");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				tessellationLength = s.Serialize<float>(tessellationLength, name: "tessellationLength");
				parentBone = s.SerializeObject<StringID>(parentBone, name: "parentBone");
				parentBoneEnd = s.Serialize<bool>(parentBoneEnd, name: "parentBoneEnd", options: CSerializerObject.Options.BoolAsByte);
				childBone = s.SerializeObject<StringID>(childBone, name: "childBone");
				childBoneEnd = s.Serialize<bool>(childBoneEnd, name: "childBoneEnd", options: CSerializerObject.Options.BoolAsByte);
				childOrientationInfluence = s.Serialize<float>(childOrientationInfluence, name: "childOrientationInfluence");
				attachToChild = s.Serialize<bool>(attachToChild, name: "attachToChild", options: CSerializerObject.Options.BoolAsByte);
			} else {
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					texture = s.SerializeObject<Path>(texture, name: "texture");
				}
				material = s.SerializeObject<GFXMaterialSerializable>(material, name: "material");
				widthStart = s.Serialize<float>(widthStart, name: "widthStart");
				widthEnd = s.Serialize<float>(widthEnd, name: "widthEnd");
				tileLength = s.Serialize<float>(tileLength, name: "tileLength");
				uvScrollSpeed = s.Serialize<float>(uvScrollSpeed, name: "uvScrollSpeed");
				zOffset = s.Serialize<float>(zOffset, name: "zOffset");
				tessellationLength = s.Serialize<float>(tessellationLength, name: "tessellationLength");
				parentBone = s.SerializeObject<StringID>(parentBone, name: "parentBone");
				childBone = s.SerializeObject<StringID>(childBone, name: "childBone");
				childOrientationInfluence = s.Serialize<float>(childOrientationInfluence, name: "childOrientationInfluence");
				attachToChild = s.Serialize<bool>(attachToChild, name: "attachToChild");
			}
		}
		public override uint? ClassCRC => 0x807DC33A;
	}
}

