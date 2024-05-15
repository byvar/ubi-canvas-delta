namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class FXControllerComponent : ActorComponent {
		public bool allowBusMixEvents;
		public bool allowMusicEvents;
		public CMap<StringID, StringID> FXCurrentSwitchRules;
		public int triggerFXisActive;
		public StringID triggerFx;
		public StringID defaultFx;
		public CListO<StringID> defaultFxList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					triggerFXisActive = s.Serialize<int>(triggerFXisActive, name: "triggerFXisActive");
				}
			} else if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					triggerFx = s.SerializeObject<StringID>(triggerFx, name: "triggerFx");
					defaultFx = s.SerializeObject<StringID>(defaultFx, name: "defaultFx");
					allowBusMixEvents = s.Serialize<bool>(allowBusMixEvents, name: "allowBusMixEvents", options: CSerializerObject.Options.BoolAsByte);
					allowMusicEvents = s.Serialize<bool>(allowMusicEvents, name: "allowMusicEvents", options: CSerializerObject.Options.BoolAsByte);
				}
			} else if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					triggerFx = s.SerializeObject<StringID>(triggerFx, name: "triggerFx");
					defaultFx = s.SerializeObject<StringID>(defaultFx, name: "defaultFx");
					defaultFxList = s.SerializeObject<CListO<StringID>>(defaultFxList, name: "defaultFxList");
					allowBusMixEvents = s.Serialize<bool>(allowBusMixEvents, name: "allowBusMixEvents", options: CSerializerObject.Options.BoolAsByte);
					allowMusicEvents = s.Serialize<bool>(allowMusicEvents, name: "allowMusicEvents", options: CSerializerObject.Options.BoolAsByte);
				}
			} else if (s.Settings.Game == Game.VH) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					allowBusMixEvents = s.Serialize<bool>(allowBusMixEvents, name: "allowBusMixEvents");
					allowMusicEvents = s.Serialize<bool>(allowMusicEvents, name: "allowMusicEvents");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					allowBusMixEvents = s.Serialize<bool>(allowBusMixEvents, name: "allowBusMixEvents");
					allowMusicEvents = s.Serialize<bool>(allowMusicEvents, name: "allowMusicEvents");
				}
				if (s.HasFlags(SerializeFlags.Group_PropertyEdit)) {
					FXCurrentSwitchRules = s.SerializeObject<CMap<StringID, StringID>>(FXCurrentSwitchRules, name: "FXCurrentSwitchRules");
				}
			}
		}
		public override uint? ClassCRC => 0x8D4FFFB6;
	}
}

