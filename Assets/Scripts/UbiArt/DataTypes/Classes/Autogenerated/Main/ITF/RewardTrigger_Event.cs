namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class RewardTrigger_Event : RewardTrigger_Base {
		public StringID eventID;
		public Enum_conditionType conditionType;
		public float floatValue;
		public StringID stringValue;
		public CArrayO<StringID> stringArrayValue;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			eventID = s.SerializeObject<StringID>(eventID, name: "eventID");
			if (!(s.Settings.Game == Game.RL && s.Settings.Platform == GamePlatform.Vita)) {
				conditionType = s.Serialize<Enum_conditionType>(conditionType, name: "conditionType");
				floatValue = s.Serialize<float>(floatValue, name: "floatValue");
				stringValue = s.SerializeObject<StringID>(stringValue, name: "stringValue");
				stringArrayValue = s.SerializeObject<CArrayO<StringID>>(stringArrayValue, name: "stringArrayValue");
			}
		}
		public enum Enum_conditionType {
			[Serialize("Triggered"       )] Triggered = 0,
			[Serialize("EqualsToString"  )] EqualsToString = 1,
			[Serialize("GreaterThanFloat")] GreaterThanFloat = 2,
			[Serialize("LesserThanFloat" )] LesserThanFloat = 3,
		}
		public override uint? ClassCRC => 0x30704FC0;
	}
}

