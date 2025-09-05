namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRoamingVertical_Template : COL_BTActionBase_Template {
		public StringID animUp;
		public StringID animDown;
		public StringID animIdle;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animUp = s.SerializeObject<StringID>(animUp, name: "animUp");
			animDown = s.SerializeObject<StringID>(animDown, name: "animDown");
			animIdle = s.SerializeObject<StringID>(animIdle, name: "animIdle");
		}
		public override uint? ClassCRC => 0x8AF360A0;
	}
}

