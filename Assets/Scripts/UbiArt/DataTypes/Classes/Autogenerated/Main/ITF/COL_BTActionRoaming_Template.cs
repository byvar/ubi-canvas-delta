namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRoaming_Template : COL_BTActionBase_Template {
		public StringID animWalk;
		public StringID animUTurn;
		public StringID animIdle;
		public float idleDuration;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animWalk = s.SerializeObject<StringID>(animWalk, name: "animWalk");
			animUTurn = s.SerializeObject<StringID>(animUTurn, name: "animUTurn");
			animIdle = s.SerializeObject<StringID>(animIdle, name: "animIdle");
			idleDuration = s.Serialize<float>(idleDuration, name: "idleDuration");
		}
		public override uint? ClassCRC => 0x66A974F5;
	}
}

