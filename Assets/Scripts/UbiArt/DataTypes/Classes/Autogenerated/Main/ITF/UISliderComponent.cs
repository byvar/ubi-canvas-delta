namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RAVersion)]
	public partial class UISliderComponent : UIItemBasic {
		public StringID itemCursorID;
		public StringID itemBarID;
		public float slideBarAbsoluteWidth;
		public float slideBarWidth;
		public float fixPosX;
		public float fixPosY;
		public float maxValue;
		public float cursorSpeed;
		public bool useSliderOrigin;
		public bool verticalNotHorizontal;
		public bool needPressValidateToSlide;
		public bool enableValueText;
		public float float__2;
		public float float__3;
		public float float__4;
		public float float__5;
		public float float__6;
		public bool bool__7;
		public bool bool__8;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion <= EngineVersion.RO) {
			} else if (s.Settings.Game == Game.RL) {
				slideBarWidth = s.Serialize<float>(slideBarWidth, name: "slideBarWidth");
				fixPosX = s.Serialize<float>(fixPosX, name: "fixPosX");
				fixPosY = s.Serialize<float>(fixPosY, name: "fixPosY");
			} else if (s.Settings.Game == Game.VH) {
				itemCursorID = s.SerializeObject<StringID>(itemCursorID, name: "itemCursorID");
				itemBarID = s.SerializeObject<StringID>(itemBarID, name: "itemBarID");
				float__2 = s.Serialize<float>(float__2, name: "float__2");
				float__3 = s.Serialize<float>(float__3, name: "float__3");
				float__4 = s.Serialize<float>(float__4, name: "float__4");
				float__5 = s.Serialize<float>(float__5, name: "float__5");
				float__6 = s.Serialize<float>(float__6, name: "float__6");
				bool__7 = s.Serialize<bool>(bool__7, name: "bool__7");
				bool__8 = s.Serialize<bool>(bool__8, name: "bool__8");
			} else {
				itemCursorID = s.SerializeObject<StringID>(itemCursorID, name: "itemCursorID");
				itemBarID = s.SerializeObject<StringID>(itemBarID, name: "itemBarID");
				slideBarAbsoluteWidth = s.Serialize<float>(slideBarAbsoluteWidth, name: "slideBarAbsoluteWidth");
				slideBarWidth = s.Serialize<float>(slideBarWidth, name: "slideBarWidth");
				fixPosX = s.Serialize<float>(fixPosX, name: "fixPosX");
				fixPosY = s.Serialize<float>(fixPosY, name: "fixPosY");
				maxValue = s.Serialize<float>(maxValue, name: "maxValue");
				cursorSpeed = s.Serialize<float>(cursorSpeed, name: "cursorSpeed");
				useSliderOrigin = s.Serialize<bool>(useSliderOrigin, name: "useSliderOrigin");
				verticalNotHorizontal = s.Serialize<bool>(verticalNotHorizontal, name: "verticalNotHorizontal");
				needPressValidateToSlide = s.Serialize<bool>(needPressValidateToSlide, name: "needPressValidateToSlide");
				enableValueText = s.Serialize<bool>(enableValueText, name: "enableValueText");
			}
		}
		public override uint? ClassCRC => 0x0E148AF2;
	}
}

