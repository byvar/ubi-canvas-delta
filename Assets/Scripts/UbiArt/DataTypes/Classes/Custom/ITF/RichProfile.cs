namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RichProfile : CSerializable {
		public int pid;
		public string name;
		public uint statusIcon;
		public int country;
		public uint globalMedalsRank;
		public uint globalMedalsMaxRank;
		public uint diamondMedals;
		public uint goldMedals;
		public uint silverMedals;
		public uint bronzeMedals;
		public PlayerStats playerStats;
		public uint costume;
		public uint totalChallengePlayed;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				pid = s.Serialize<int>(pid, name: "pid");
				name = s.Serialize<string>(name, name: "name");
				statusIcon = s.Serialize<uint>(statusIcon, name: "statusIcon");
				country = s.Serialize<int>(country, name: "country");
				globalMedalsRank = s.Serialize<uint>(globalMedalsRank, name: "globalMedalsRank");
				globalMedalsMaxRank = s.Serialize<uint>(globalMedalsMaxRank, name: "globalMedalsMaxRank");
				diamondMedals = s.Serialize<uint>(diamondMedals, name: "diamondMedals");
				goldMedals = s.Serialize<uint>(goldMedals, name: "goldMedals");
				silverMedals = s.Serialize<uint>(silverMedals, name: "silverMedals");
				bronzeMedals = s.Serialize<uint>(bronzeMedals, name: "bronzeMedals");
				playerStats = s.SerializeObject<PlayerStats>(playerStats, name: "playerStats");
				costume = s.Serialize<uint>(costume, name: "costume");
				totalChallengePlayed = s.Serialize<uint>(totalChallengePlayed, name: "totalChallengePlayed");
			}
		}
	}
}
