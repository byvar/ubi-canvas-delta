namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class UITextManager_Template : TemplateObj {
		public CListO<UITextManager_Template.ActorIcon> actorIcons;
		public float iconSize;
		public float iconYOffset;
		public float iconXOffset;
		public Path buttonPath;
		public CListP<string> buttonNames;
		public Path gpePath;
		public CListP<string> gpeNames;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO) {
				iconSize = s.Serialize<float>(iconSize, name: "iconSize");
				iconYOffset = s.Serialize<float>(iconYOffset, name: "iconYOffset");
				iconXOffset = s.Serialize<float>(iconXOffset, name: "iconXOffset");
				buttonPath = s.SerializeObject<Path>(buttonPath, name: "buttonPath");
				buttonNames = s.SerializeObject<CListP<string>>(buttonNames, name: "buttonNames");
				gpePath = s.SerializeObject<Path>(gpePath, name: "gpePath");
				gpeNames = s.SerializeObject<CListP<string>>(gpeNames, name: "gpeNames");
			} else {
				actorIcons = s.SerializeObject<CListO<UITextManager_Template.ActorIcon>>(actorIcons, name: "actorIcons");
			}
		}
		[Games(GameFlags.VH | GameFlags.RA)]
		public partial class ActorIcon : CSerializable {
			public StringID iconName;
			public Path iconPath;
			public float fontSize;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				iconName = s.SerializeObject<StringID>(iconName, name: "iconName");
				iconPath = s.SerializeObject<Path>(iconPath, name: "iconPath");
				fontSize = s.Serialize<float>(fontSize, name: "fontSize");
			}
		}
		public override uint? ClassCRC => 0xAA368C44;
	}
}

