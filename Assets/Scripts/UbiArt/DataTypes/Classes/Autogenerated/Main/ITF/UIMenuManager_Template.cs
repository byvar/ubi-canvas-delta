namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class UIMenuManager_Template : TemplateObj {
		public CListO<UIMenuManager_Template.MenuInfo> menuInfos;
		public bool useRemoteUI;
		public StringID defaultValidInput;
		public StringID defaultActionInput;
		public StringID defaultBackInput;
		public Placeholder menuSounds;
		public int showESRBMenu;
		public StringID defaultBackInputSecondary;
		public StringID defaultLeftButtonInput;
		public StringID defaultRightButtonInput;
		public StringID defaultOtherButtonInput;
		public float initialInputDelay;
		public float inputDelayWhenActive;
		public float inputStickDeadZone;
		public int minimumDepthToHideDialogBalloons;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL || s.Settings.Game == Game.VH) {
				menuInfos = s.SerializeObject<CListO<UIMenuManager_Template.MenuInfo>>(menuInfos, name: "menuInfos");
				useRemoteUI = s.Serialize<bool>(useRemoteUI, name: "useRemoteUI");
				defaultValidInput = s.SerializeObject<StringID>(defaultValidInput, name: "defaultValidInput");
				defaultBackInput = s.SerializeObject<StringID>(defaultBackInput, name: "defaultBackInput");
			} else if (s.Settings.Game == Game.COL) {
				menuSounds = s.SerializeObject<Placeholder>(menuSounds, name: "menuSounds");
				showESRBMenu = s.Serialize<int>(showESRBMenu, name: "showESRBMenu");
				useRemoteUI = s.Serialize<bool>(useRemoteUI, name: "useRemoteUI");
				defaultValidInput = s.SerializeObject<StringID>(defaultValidInput, name: "defaultValidInput");
				defaultActionInput = s.SerializeObject<StringID>(defaultActionInput, name: "defaultActionInput");
				defaultBackInput = s.SerializeObject<StringID>(defaultBackInput, name: "defaultBackInput");
				defaultBackInputSecondary = s.SerializeObject<StringID>(defaultBackInputSecondary, name: "defaultBackInputSecondary");
				defaultLeftButtonInput = s.SerializeObject<StringID>(defaultLeftButtonInput, name: "defaultLeftButtonInput");
				defaultRightButtonInput = s.SerializeObject<StringID>(defaultRightButtonInput, name: "defaultRightButtonInput");
				defaultOtherButtonInput = s.SerializeObject<StringID>(defaultOtherButtonInput, name: "defaultOtherButtonInput");
				initialInputDelay = s.Serialize<float>(initialInputDelay, name: "initialInputDelay");
				inputDelayWhenActive = s.Serialize<float>(inputDelayWhenActive, name: "inputDelayWhenActive");
				inputStickDeadZone = s.Serialize<float>(inputStickDeadZone, name: "inputStickDeadZone");
				minimumDepthToHideDialogBalloons = s.Serialize<int>(minimumDepthToHideDialogBalloons, name: "minimumDepthToHideDialogBalloons");
			} else {
				menuInfos = s.SerializeObject<CListO<UIMenuManager_Template.MenuInfo>>(menuInfos, name: "menuInfos");
				useRemoteUI = s.Serialize<bool>(useRemoteUI, name: "useRemoteUI");
				defaultValidInput = s.SerializeObject<StringID>(defaultValidInput, name: "defaultValidInput");
				defaultActionInput = s.SerializeObject<StringID>(defaultActionInput, name: "defaultActionInput");
				defaultBackInput = s.SerializeObject<StringID>(defaultBackInput, name: "defaultBackInput");
			}
		}
		[Games(GameFlags.VH | GameFlags.RA)]
		public partial class MenuInfo : CSerializable {
			public Path path;
			public PathRef optionalPath;
			public int depth;
			public uint flags;
			public bool bool__3;
			public Path Path__4;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				if (s.Settings.Game == Game.RL) {
					path = s.SerializeObject<Path>(path, name: "path");
					depth = s.Serialize<int>(depth, name: "depth");
					flags = s.Serialize<uint>(flags, name: "flags");
				} else if (s.Settings.Game == Game.VH) {
					path = s.SerializeObject<Path>(path, name: "path");
					depth = s.Serialize<int>(depth, name: "depth");
					flags = s.Serialize<uint>(flags, name: "flags");
					bool__3 = s.Serialize<bool>(bool__3, name: "bool__3");
					Path__4 = s.SerializeObject<Path>(Path__4, name: "Path__4");
				} else {
					path = s.SerializeObject<Path>(path, name: "path");
					optionalPath = s.SerializeObject<PathRef>(optionalPath, name: "optionalPath");
					depth = s.Serialize<int>(depth, name: "depth");
					flags = s.Serialize<uint>(flags, name: "flags");
				}
			}
		}
		public override uint? ClassCRC => 0x8D4F5267;
	}
}

