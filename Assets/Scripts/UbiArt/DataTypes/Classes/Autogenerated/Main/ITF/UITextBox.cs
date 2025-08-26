namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class UITextBox : UIComponent {
		public uint style;
		public Vec2d offset;
		public float depth;
		public Vec2d scale = Vec2d.One;
		public Vec2d area = new Vec2d(-1,-1);
		public float maxWidth = -1f;
		public string rawText;
		public LocalisationId locId;
		public bool unsecureSource;
		public Color overridingColor;
		public Enum_GlobalScissor GlobalScissor;
		public bool scaleToMatchWithArea { get => scaleToMatchWithAreaType != FontTextArea.ScaleMatchType.NoScaleMatch; set => scaleToMatchWithAreaType = value ? FontTextArea.ScaleMatchType.ScaleMatchOneLine : FontTextArea.ScaleMatchType.NoScaleMatch; }
		public float autoScrollSpeed;
		public float autoScrollWaitTime;
		public float autoScrollFirstWaitTime;
		public bool autoScrollLoop;
		public AutoScroll autoScrollMode;
		public AutoScrollUpdate autoScrollUpdate;
		public uint autoScrollLoopGap = 1;
		public FONT_ALIGN overridingHAlignment = FONT_ALIGN.NONE;
		public FONT_ALIGN2 overridingHAlignment2 = FONT_ALIGN2.NONE;
		public FONT overridingVAlignment = FONT.ALIGN_NONE;
		public AREA_ANCHOR overridingAnchor = AREA_ANCHOR.NONE;
		public uint ViewportVisibility = 0xFFFF;
		public bool AdaptToLangage;
		public bool DontResizeForChinese;
		public FontTextArea.ScaleMatchType scaleToMatchWithAreaType;

		public bool bool__12;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion == EngineVersion.RO) {
			} else if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					style = s.Serialize<uint>(style, name: "style");
					offset = s.SerializeObject<Vec2d>(offset, name: "offset");
					depth = s.Serialize<float>(depth, name: "depth");
					scale = s.SerializeObject<Vec2d>(scale, name: "scale");
					area = s.SerializeObject<Vec2d>(area, name: "area");
					maxWidth = s.Serialize<float>(maxWidth, name: "maxWidth");
					rawText = s.Serialize<string>(rawText, name: "rawText");
					locId = s.SerializeObject<LocalisationId>(locId, name: "locId");
					overridingColor = s.SerializeObject<Color>(overridingColor, name: "overridingColor");
					scaleToMatchWithArea = s.Serialize<bool>(scaleToMatchWithArea, name: "scaleToMatchWithArea");
					autoScrollSpeed = s.Serialize<float>(autoScrollSpeed, name: "autoScrollSpeed");
					autoScrollWaitTime = s.Serialize<float>(autoScrollWaitTime, name: "autoScrollWaitTime");
					overridingHAlignment2 = s.Serialize<FONT_ALIGN2>(overridingHAlignment2, name: "overridingHAlignment");
					overridingVAlignment = s.Serialize<FONT>(overridingVAlignment, name: "overridingVAlignment");
					overridingAnchor = s.Serialize<AREA_ANCHOR>(overridingAnchor, name: "overridingAnchor");
				}
			} else if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					style = s.Serialize<uint>(style, name: "style");
					offset = s.SerializeObject<Vec2d>(offset, name: "offset");
					depth = s.Serialize<float>(depth, name: "depth");
					scale = s.SerializeObject<Vec2d>(scale, name: "scale");
					area = s.SerializeObject<Vec2d>(area, name: "area");
					maxWidth = s.Serialize<float>(maxWidth, name: "maxWidth");
					rawText = s.Serialize<string>(rawText, name: "rawText");
					locId = (LocalisationId)s.SerializeObject<StringID>((StringID)locId, name: "locId");
					overridingColor = s.SerializeObject<Color>(overridingColor, name: "overridingColor");
					scaleToMatchWithArea = s.Serialize<bool>(scaleToMatchWithArea, name: "scaleToMatchWithArea");
					autoScrollSpeed = s.Serialize<float>(autoScrollSpeed, name: "autoScrollSpeed");
					autoScrollWaitTime = s.Serialize<float>(autoScrollWaitTime, name: "autoScrollWaitTime");
					overridingHAlignment2 = s.Serialize<FONT_ALIGN2>(overridingHAlignment2, name: "overridingHAlignment");
					overridingVAlignment = s.Serialize<FONT>(overridingVAlignment, name: "overridingVAlignment");
					overridingAnchor = s.Serialize<AREA_ANCHOR>(overridingAnchor, name: "overridingAnchor");
				}
			} else if (s.Settings.Game == Game.VH) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					style = s.Serialize<uint>(style, name: "style");
					offset = s.SerializeObject<Vec2d>(offset, name: "offset");
					depth = s.Serialize<float>(depth, name: "depth");
					scale = s.SerializeObject<Vec2d>(scale, name: "scale");
					area = s.SerializeObject<Vec2d>(area, name: "area");
					maxWidth = s.Serialize<float>(maxWidth, name: "maxWidth");
					rawText = s.Serialize<string>(rawText, name: "rawText");
					locId = s.SerializeObject<LocalisationId>(locId, name: "locId");
					overridingColor = s.SerializeObject<Color>(overridingColor, name: "overridingColor");
					if (s.HasFlags(SerializeFlags.Deprecate)) {
						scaleToMatchWithArea = s.Serialize<bool>(scaleToMatchWithArea, name: "scaleToMatchWithArea");
					}
					scaleToMatchWithAreaType = s.Serialize<FontTextArea.ScaleMatchType>(scaleToMatchWithAreaType, name: "scaleToMatchWithArea");
					autoScrollSpeed = s.Serialize<float>(autoScrollSpeed, name: "autoScrollSpeed");
					autoScrollWaitTime = s.Serialize<float>(autoScrollWaitTime, name: "autoScrollWaitTime");
					bool__12 = s.Serialize<bool>(bool__12, name: "bool__12");
					autoScrollLoopGap = s.Serialize<uint>(autoScrollLoopGap, name: "autoScrollLoopGap");
					overridingHAlignment = s.Serialize<FONT_ALIGN>(overridingHAlignment, name: "overridingHAlignment");
					overridingVAlignment = s.Serialize<FONT>(overridingVAlignment, name: "overridingVAlignment");
					overridingAnchor = s.Serialize<AREA_ANCHOR>(overridingAnchor, name: "overridingAnchor");
				}
			} else {
				style = s.Serialize<uint>(style, name: "style");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				depth = s.Serialize<float>(depth, name: "depth");
				scale = s.SerializeObject<Vec2d>(scale, name: "scale");
				area = s.SerializeObject<Vec2d>(area, name: "area");
				maxWidth = s.Serialize<float>(maxWidth, name: "maxWidth");
				rawText = s.Serialize<string>(rawText, name: "rawText");
				locId = s.SerializeObject<LocalisationId>(locId, name: "locId");
				unsecureSource = s.Serialize<bool>(unsecureSource, name: "unsecureSource");
				overridingColor = s.SerializeObject<Color>(overridingColor, name: "overridingColor");
				GlobalScissor = s.Serialize<Enum_GlobalScissor>(GlobalScissor, name: "GlobalScissor");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					scaleToMatchWithArea = s.Serialize<bool>(scaleToMatchWithArea, name: "scaleToMatchWithArea");
				}
				scaleToMatchWithAreaType = s.Serialize<FontTextArea.ScaleMatchType>(scaleToMatchWithAreaType, name: "scaleToMatchWithArea");
				autoScrollSpeed = s.Serialize<float>(autoScrollSpeed, name: "autoScrollSpeed");
				autoScrollWaitTime = s.Serialize<float>(autoScrollWaitTime, name: "autoScrollWaitTime");
				autoScrollFirstWaitTime = s.Serialize<float>(autoScrollFirstWaitTime, name: "autoScrollFirstWaitTime");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					autoScrollLoop = s.Serialize<bool>(autoScrollLoop, name: "autoScrollLoop");
				}
				autoScrollMode = s.Serialize<AutoScroll>(autoScrollMode, name: "autoScrollMode");
				autoScrollUpdate = s.Serialize<AutoScrollUpdate>(autoScrollUpdate, name: "autoScrollUpdate");
				autoScrollLoopGap = s.Serialize<uint>(autoScrollLoopGap, name: "autoScrollLoopGap");
				overridingHAlignment = s.Serialize<FONT_ALIGN>(overridingHAlignment, name: "overridingHAlignment");
				overridingVAlignment = s.Serialize<FONT>(overridingVAlignment, name: "overridingVAlignment");
				overridingAnchor = s.Serialize<AREA_ANCHOR>(overridingAnchor, name: "overridingAnchor");
				ViewportVisibility = s.Serialize<uint>(ViewportVisibility, name: "ViewportVisibility");
				AdaptToLangage = s.Serialize<bool>(AdaptToLangage, name: "AdaptToLangage");
				DontResizeForChinese = s.Serialize<bool>(DontResizeForChinese, name: "DontResizeForChinese");
			}
		}
		public enum Enum_GlobalScissor {
			[Serialize("NONE"     )] NONE = 0,
			[Serialize("SCISSOR_1")] SCISSOR_1 = 1,
			[Serialize("SCISSOR_2")] SCISSOR_2 = 2,
			[Serialize("SCISSOR_3")] SCISSOR_3 = 3,
		}
		public enum AutoScroll {
			[Serialize("AutoScroll_None"        )] None = 0,
			[Serialize("AutoScroll_BackAndForth")] BackAndForth = 1,
			[Serialize("AutoScroll_Loop"        )] Loop = 2,
			[Serialize("AutoScroll_LoopIfNeeded")] LoopIfNeeded = 3,
		}
		public enum AutoScrollUpdate {
			[Serialize("AutoScrollUpdate_Enable" )] Enable = 0,
			[Serialize("AutoScrollUpdate_Freeze" )] Freeze = 1,
			[Serialize("AutoScrollUpdate_Disable")] Disable = 2,
		}
		public enum FONT_ALIGN {
			[Serialize("FONT_ALIGN_NONE"   )] NONE = -1,
			[Serialize("FONT_ALIGN_LEFT"   )] LEFT = 0,
			[Serialize("FONT_ALIGN_CENTER" )] CENTER = 1,
			[Serialize("FONT_ALIGN_RIGHT"  )] RIGHT = 2,
			[Serialize("FONT_ALIGN_JUSTIFY")] JUSTIFY = 3,
		}
		public enum FONT {
			[Serialize("FONT_ALIGN_NONE"   )] ALIGN_NONE = -1,
			[Serialize("FONT_VALIGN_TOP"   )] VALIGN_TOP = 0,
			[Serialize("FONT_VALIGN_MIDDLE")] VALIGN_MIDDLE = 1,
			[Serialize("FONT_VALIGN_BOTTOM")] VALIGN_BOTTOM = 2,
		}
		public enum AREA_ANCHOR {
			[Serialize("AREA_ANCHOR_NONE"         )] NONE = -1,
			[Serialize("AREA_ANCHOR_TOP_LEFT"     )] TOP_LEFT = 0,
			[Serialize("AREA_ANCHOR_MIDDLE_CENTER")] MIDDLE_CENTER = 1,
			[Serialize("AREA_ANCHOR_MIDDLE_LEFT"  )] MIDDLE_LEFT = 2,
			[Serialize("AREA_ANCHOR_MIDDLE_RIGHT" )] MIDDLE_RIGHT = 3,
			[Serialize("AREA_ANCHOR_TOP_CENTER"   )] TOP_CENTER = 4,
			[Serialize("AREA_ANCHOR_TOP_RIGHT"    )] TOP_RIGHT = 5,
			[Serialize("AREA_ANCHOR_BOTTOM_CENTER")] BOTTOM_CENTER = 6,
			[Serialize("AREA_ANCHOR_BOTTOM_LEFT"  )] BOTTOM_LEFT = 7,
			[Serialize("AREA_ANCHOR_BOTTOM_RIGHT" )] BOTTOM_RIGHT = 8,
		}
		public enum FONT_ALIGN2 {
			[Serialize("FONT_ALIGN_NONE"   )] NONE = -1,
			[Serialize("FONT_ALIGN_LEFT"   )] LEFT = 0,
			[Serialize("FONT_ALIGN_CENTER" )] CENTER = 1,
			[Serialize("FONT_ALIGN_RIGHT"  )] RIGHT = 2,
		}
		public override uint? ClassCRC => 0xD10CBEED;
	}
}

