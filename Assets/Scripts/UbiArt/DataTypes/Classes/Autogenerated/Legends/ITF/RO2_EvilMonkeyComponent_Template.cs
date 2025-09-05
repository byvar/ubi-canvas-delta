namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EvilMonkeyComponent_Template : RO2_EnemyBTAIComponent_Template {
		public uint faction_monkey;
		public float squashDeathPenetration_monkey;
		public float timeFight_monkey;
		public int disabledPhys_monkey;
		public int withShield_monkey;
		public StringID colShield_monkey;
		public StringID colShield2_monkey;
		public float timeBlockingContact_monkey;
		public int withStilts;
		public StringID colStilt01;
		public StringID colStilt02;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			faction_monkey = s.Serialize<uint>(faction_monkey, name: "faction");
			squashDeathPenetration_monkey = s.Serialize<float>(squashDeathPenetration_monkey, name: "squashDeathPenetration");
			timeFight_monkey = s.Serialize<float>(timeFight_monkey, name: "timeFight");
			disabledPhys_monkey = s.Serialize<int>(disabledPhys_monkey, name: "disabledPhys");
			withShield_monkey = s.Serialize<int>(withShield_monkey, name: "withShield");
			colShield_monkey = s.SerializeObject<StringID>(colShield_monkey, name: "colShield");
			colShield2_monkey = s.SerializeObject<StringID>(colShield2_monkey, name: "colShield2");
			timeBlockingContact_monkey = s.Serialize<float>(timeBlockingContact_monkey, name: "timeBlockingContact");
			withStilts = s.Serialize<int>(withStilts, name: "withStilts");
			colStilt01 = s.SerializeObject<StringID>(colStilt01, name: "colStilt01");
			colStilt02 = s.SerializeObject<StringID>(colStilt02, name: "colStilt02");
		}
		public override uint? ClassCRC => 0x7E98C9A8;
	}
}

