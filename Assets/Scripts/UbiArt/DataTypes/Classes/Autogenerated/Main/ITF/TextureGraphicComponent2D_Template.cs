namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class TextureGraphicComponent2D_Template : GraphicComponent_Template {
		public Path texture;
		public uint rank;
		public CArrayO<TexLanguageOverride> texOverrides; // ONLY in the DEMO!
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			texture = s.SerializeObject<Path>(texture, name: "texture");
			rank = s.Serialize<uint>(rank, name: "rank");
			if (s.Settings.Game == Game.RO && s.Settings.IsDemo) {
				texOverrides = s.SerializeObject<CArrayO<TexLanguageOverride>>(texOverrides, name: "texOverrides");
			}
		}
		public override uint? ClassCRC => 0xA2ACC46E;

		[Games(GameFlags.RO)]
		public partial class TexLanguageOverride : CSerializable {
			public int language;
			public Path texture;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				language = s.Serialize<int>(language, name: "language");
				texture = s.SerializeObject<Path>(texture, name: "texture");
			}
		}
	}
}

