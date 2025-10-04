namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class RewardContainer_Template : TemplateObj {
		public CListO<RewardDetail> rewards;
		public RewardStatHandler statsHandler;
		public bool isSilent;
		public Generic<RewardsUserData> rewardsUserData;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL || s.Settings.Game == Game.VH) {
				rewards = s.SerializeObject<CListO<RewardDetail>>(rewards, name: "rewards");
				isSilent = s.Serialize<bool>(isSilent, name: "isSilent");
			} else if (s.Settings.Game == Game.COL) {
				rewards = s.SerializeObject<CListO<RewardDetail>>(rewards, name: "rewards");
				rewardsUserData = s.SerializeObject<Generic<RewardsUserData>>(rewardsUserData, name: "rewardsUserData");
				isSilent = s.Serialize<bool>(isSilent, name: "isSilent");
			} else {
				rewards = s.SerializeObject<CListO<RewardDetail>>(rewards, name: "rewards");
				statsHandler = s.SerializeObject<RewardStatHandler>(statsHandler, name: "statsHandler");
				isSilent = s.Serialize<bool>(isSilent, name: "isSilent");
			}
		}
		public override uint? ClassCRC => 0x3ABE1DA8;
	}
}

