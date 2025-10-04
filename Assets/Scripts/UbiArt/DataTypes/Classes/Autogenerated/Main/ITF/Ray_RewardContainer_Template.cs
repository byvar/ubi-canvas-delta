namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_RewardContainer_Template : TemplateObj {
		public CListO<Ray_RewardDetail> rewards;
		public int isSilent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			rewards = s.SerializeObject<CListO<Ray_RewardDetail>>(rewards, name: "rewards");
			isSilent = s.Serialize<int>(isSilent, name: "isSilent");
		}
		public override uint? ClassCRC => 0xED6F1313;
	}
}

