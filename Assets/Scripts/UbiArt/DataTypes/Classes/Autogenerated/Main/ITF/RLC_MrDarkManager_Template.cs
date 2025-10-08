namespace UbiArt.ITF {
	[Games(GameFlags.RM)]
	public partial class RLC_MrDarkManager_Template : CSerializable {
		public Path scoreRecapPath;
		public CArrayP<uint> nonLethalLevelsWorldIndexes;
		public CArrayP<uint> nonLethalLevelsMapIndexes;
		public CArrayP<uint> firstHalfLevelsWorldIndexes;
		public CArrayP<uint> firstHalfLevelsMapIndexes;
		public CArrayP<uint> secondHalfLevelsWorldIndexes;
		public CArrayP<uint> secondHalfLevelsMapIndexes;
		public CArrayP<uint> bossAndBonusLevelsWorldIndexes;
		public CArrayP<uint> bossAndBonusLevelsMapIndexes;
		public Placeholder settingsPerAwesomenessLevelChallengeReach;
		public Placeholder settingsPerAwesomenessLevelChallengeLums;
		public Placeholder settingsPerAwesomenessLevelAdventureFinish;
		public Placeholder settingsPerAwesomenessLevelAdventureFind;
		public Placeholder settingsPerAwesomenessLevelMusicalTasks;
		public Placeholder settingsPerAwesomenessLevelMarathonTasks;
		public byte numberMapsMusical;
		public CArrayP<string> modifiersUserFriendly;
		public CArrayP<string> challengeModifiersUserFriendly;
		public CArrayP<string> objectsToFindUserFriendly;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			scoreRecapPath = s.SerializeObject<Path>(scoreRecapPath, name: "scoreRecapPath");
			nonLethalLevelsWorldIndexes = s.SerializeObject<CArrayP<uint>>(nonLethalLevelsWorldIndexes, name: "nonLethalLevelsWorldIndexes");
			nonLethalLevelsMapIndexes = s.SerializeObject<CArrayP<uint>>(nonLethalLevelsMapIndexes, name: "nonLethalLevelsMapIndexes");
			firstHalfLevelsWorldIndexes = s.SerializeObject<CArrayP<uint> > (firstHalfLevelsWorldIndexes, name: "firstHalfLevelsWorldIndexes");
			firstHalfLevelsMapIndexes = s.SerializeObject<CArrayP<uint>>(firstHalfLevelsMapIndexes, name: "firstHalfLevelsMapIndexes");
			secondHalfLevelsWorldIndexes = s.SerializeObject<CArrayP<uint>>(secondHalfLevelsWorldIndexes, name: "secondHalfLevelsWorldIndexes");
			secondHalfLevelsMapIndexes = s.SerializeObject<CArrayP<uint>>(secondHalfLevelsMapIndexes, name: "secondHalfLevelsMapIndexes");
			bossAndBonusLevelsWorldIndexes = s.SerializeObject<CArrayP<uint>>(bossAndBonusLevelsWorldIndexes, name: "bossAndBonusLevelsWorldIndexes");
			bossAndBonusLevelsMapIndexes = s.SerializeObject<CArrayP<uint>>(bossAndBonusLevelsMapIndexes, name: "bossAndBonusLevelsMapIndexes");
			settingsPerAwesomenessLevelChallengeReach = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelChallengeReach, name: "settingsPerAwesomenessLevelChallengeReach");
			settingsPerAwesomenessLevelChallengeLums = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelChallengeLums, name: "settingsPerAwesomenessLevelChallengeLums");
			settingsPerAwesomenessLevelAdventureFinish = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelAdventureFinish, name: "settingsPerAwesomenessLevelAdventureFinish");
			settingsPerAwesomenessLevelAdventureFind = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelAdventureFind, name: "settingsPerAwesomenessLevelAdventureFind");
			settingsPerAwesomenessLevelMusicalTasks = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelMusicalTasks, name: "settingsPerAwesomenessLevelMusicalTasks");
			settingsPerAwesomenessLevelMarathonTasks = s.SerializeObject<Placeholder>(settingsPerAwesomenessLevelMarathonTasks, name: "settingsPerAwesomenessLevelMarathonTasks");
			numberMapsMusical = s.Serialize<byte>(numberMapsMusical, name: "numberMapsMusical");
			modifiersUserFriendly = s.SerializeObject<CArrayP<string>>(modifiersUserFriendly, name: "modifiersUserFriendly");
			challengeModifiersUserFriendly = s.SerializeObject<CArrayP<string>>(challengeModifiersUserFriendly, name: "challengeModifiersUserFriendly");
			objectsToFindUserFriendly = s.SerializeObject<CArrayP<string>>(objectsToFindUserFriendly, name: "objectsToFindUserFriendly");
		}
		public override uint? ClassCRC => 0xD63C0714;
	}
}

