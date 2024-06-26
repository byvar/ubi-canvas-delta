namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CinematicDialogsUI_Template : CSerializable {
		public Path dialogBackgroundTexturePath;
		public Path nameBackgroundTexturePath;
		public Placeholder dialogBackgroundTexture;
		public Placeholder nameBackgroundTexture;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				dialogBackgroundTexturePath = s.SerializeObject<Path>(dialogBackgroundTexturePath, name: "dialogBackgroundTexturePath");
				nameBackgroundTexturePath = s.SerializeObject<Path>(nameBackgroundTexturePath, name: "nameBackgroundTexturePath");
			}
			dialogBackgroundTexture = s.SerializeObject<Placeholder>(dialogBackgroundTexture, name: "dialogBackgroundTexture");
			nameBackgroundTexture = s.SerializeObject<Placeholder>(nameBackgroundTexture, name: "nameBackgroundTexture");
		}
		public override uint? ClassCRC => 0xF0DDE2DB;
	}
}

