namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class UIItemBasic : UIItem {
		public uint selectTextStyle;
		public CListO<StringID> selectAnimMeshVertex;
		public PathRef PathForMap;
		public StringID nextMenuOnValidate;
		public StringID WwiseGUID_OnValidate;
		public StringID WwiseGUID_OnValidateLocked;
		public StringID WwiseGUID_OnSelect;
		public CListO<sEventData> OnValidateEvents;
		public CListO<sEventData> OnValidateLockedEvents;
		public CListO<sEventData> OnSelectEvents;
		public EventSender WwiseOnValidate;
		public bool isUIPADListener;
		public bool isUIFruityListener;
		public bool isDisplayedWithPad;
		public bool isDisplayedWithTouch;
		public Vec2d padPointerOffset;

		public EventSender EventSender__3;
		public EventSender EventSender__4;

		public bool navigable;
		public bool doubleClickToValidate;
		public bool supportRightMouseButton;
		public bool canMouseChangeSpriteIndex;
		public uint releasedSpriteIndex;
		public uint pressedSpriteIndex;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion <= EngineVersion.RO) {
			} else if (s.Settings.Game == Game.RL) {
				if (this is UISliderComponent) return;
				selectTextStyle = s.Serialize<uint>(selectTextStyle, name: "selectTextStyle");
				selectAnimMeshVertex = s.SerializeObject<CListO<StringID>>(selectAnimMeshVertex, name: "selectAnimMeshVertex");
			} else if(s.Settings.Game == Game.COL) {
				if (this is UISliderComponent) return;
				selectTextStyle = s.Serialize<uint>(selectTextStyle, name: "selectTextStyle");
				selectAnimMeshVertex = s.SerializeObject<CListO<StringID>>(selectAnimMeshVertex, name: "selectAnimMeshVertex");
				navigable = s.Serialize<bool>(navigable, name: "navigable");
				doubleClickToValidate = s.Serialize<bool>(doubleClickToValidate, name: "doubleClickToValidate");
				supportRightMouseButton = s.Serialize<bool>(supportRightMouseButton, name: "supportRightMouseButton");
				canMouseChangeSpriteIndex = s.Serialize<bool>(canMouseChangeSpriteIndex, name: "canMouseChangeSpriteIndex");
				releasedSpriteIndex = s.Serialize<uint>(releasedSpriteIndex, name: "releasedSpriteIndex");
				pressedSpriteIndex = s.Serialize<uint>(pressedSpriteIndex, name: "pressedSpriteIndex");
			} else if (s.Settings.Game == Game.VH) {
				selectTextStyle = s.Serialize<uint>(selectTextStyle, name: "selectTextStyle");
				selectAnimMeshVertex = s.SerializeObject<CListO<StringID>>(selectAnimMeshVertex, name: "selectAnimMeshVertex");
				PathForMap = s.SerializeObject<PathRef>(PathForMap, name: "PathForMap");
				EventSender__3 = s.SerializeObject<EventSender>(EventSender__3, name: "EventSender__3");
				EventSender__4 = s.SerializeObject<EventSender>(EventSender__4, name: "EventSender__4");
			} else {
				selectTextStyle = s.Serialize<uint>(selectTextStyle, name: "selectTextStyle");
				selectAnimMeshVertex = s.SerializeObject<CListO<StringID>>(selectAnimMeshVertex, name: "selectAnimMeshVertex");
				PathForMap = s.SerializeObject<PathRef>(PathForMap, name: "PathForMap");
				if (!s.HasFlags(SerializeFlags.Group_PropertyEdit)) {
					nextMenuOnValidate = s.SerializeObject<StringID>(nextMenuOnValidate, name: "nextMenuOnValidate");
				} else {
					nextMenuOnValidate = s.SerializeChoiceListObject<StringID>(nextMenuOnValidate, name: "nextMenuOnValidate", empty: "Empty");
				}
				WwiseGUID_OnValidate = s.SerializeObject<StringID>(WwiseGUID_OnValidate, name: "WwiseGUID_OnValidate");
				WwiseGUID_OnValidateLocked = s.SerializeObject<StringID>(WwiseGUID_OnValidateLocked, name: "WwiseGUID_OnValidateLocked");
				WwiseGUID_OnSelect = s.SerializeObject<StringID>(WwiseGUID_OnSelect, name: "WwiseGUID_OnSelect");
				OnValidateEvents = s.SerializeObject<CListO<sEventData>>(OnValidateEvents, name: "OnValidateEvents");
				OnValidateLockedEvents = s.SerializeObject<CListO<sEventData>>(OnValidateLockedEvents, name: "OnValidateLockedEvents");
				OnSelectEvents = s.SerializeObject<CListO<sEventData>>(OnSelectEvents, name: "OnSelectEvents");
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					WwiseOnValidate = s.SerializeObject<EventSender>(WwiseOnValidate, name: "WwiseOnValidate");
				}
				isUIPADListener = s.Serialize<bool>(isUIPADListener, name: "isUIPADListener");
				isUIFruityListener = s.Serialize<bool>(isUIFruityListener, name: "isUIFruityListener");
				isDisplayedWithPad = s.Serialize<bool>(isDisplayedWithPad, name: "isDisplayedWithPad");
				isDisplayedWithTouch = s.Serialize<bool>(isDisplayedWithTouch, name: "isDisplayedWithTouch");
				padPointerOffset = s.SerializeObject<Vec2d>(padPointerOffset, name: "padPointerOffset");
			}
		}
		public override uint? ClassCRC => 0xEC59CF6E;
	}
}

