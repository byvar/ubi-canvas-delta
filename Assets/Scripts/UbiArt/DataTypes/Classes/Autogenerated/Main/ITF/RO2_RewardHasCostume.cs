namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_RewardHasCostume : RewardTrigger_Base {
		public CListO<StringID> costumeTags;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				costumeTags = s.SerializeObject<CListO<StringID>>(costumeTags, name: "costumeTags");
			} else {
			}
		}
		public override uint? ClassCRC => 0xCF38A4B2;
	}
}

