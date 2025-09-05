namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CinematicDialogsUI_Template : UIMenuBasic_Template {
		public Path dialogBackgroundTexturePath;
		public Path nameBackgroundTexturePath;
		public GFXMaterialSerializable dialogBackgroundTexture;
		public GFXMaterialSerializable nameBackgroundTexture;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				dialogBackgroundTexturePath = s.SerializeObject<Path>(dialogBackgroundTexturePath, name: "dialogBackgroundTexturePath");
				nameBackgroundTexturePath = s.SerializeObject<Path>(nameBackgroundTexturePath, name: "nameBackgroundTexturePath");
			}
			dialogBackgroundTexture = s.SerializeObject<GFXMaterialSerializable>(dialogBackgroundTexture, name: "dialogBackgroundTexture");
			nameBackgroundTexture = s.SerializeObject<GFXMaterialSerializable>(nameBackgroundTexture, name: "nameBackgroundTexture");
		}
		public override uint? ClassCRC => 0xF0DDE2DB;
	}
}

