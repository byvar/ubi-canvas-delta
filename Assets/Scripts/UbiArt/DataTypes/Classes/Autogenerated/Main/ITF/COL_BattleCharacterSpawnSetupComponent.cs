namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleCharacterSpawnSetupComponent : ActorComponent {
		[Description("Spawn index")]
		public int spawnIndex;
		public bool spawnFlipped;
		[Description("The team of the associated character")]
		public Enum_teamType teamType;
		[Description("ObjectId of the associated spotlight used for action/target selection")]
		public ObjectId spotlightID;
		[Description("ObjectId of the associated platform intro sequence")]
		public ObjectId platformIntroSequenceID;
		[Description("ObjectId of the associated platform death sequence")]
		public ObjectId platformDeathSequenceID;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			spawnIndex = s.Serialize<int>(spawnIndex, name: "spawnIndex");
			spawnFlipped = s.Serialize<bool>(spawnFlipped, name: "spawnFlipped", options: CSerializerObject.Options.BoolAsByte);
			teamType = s.Serialize<Enum_teamType>(teamType, name: "teamType");
			spotlightID = s.SerializeObject<ObjectId>(spotlightID, name: "spotlightID");
			platformIntroSequenceID = s.SerializeObject<ObjectId>(platformIntroSequenceID, name: "platformIntroSequenceID");
			platformDeathSequenceID = s.SerializeObject<ObjectId>(platformDeathSequenceID, name: "platformDeathSequenceID");
		}
		public enum Enum_teamType {
			[Serialize("Value_0")] Value_0 = 0,
			[Serialize("Value_1")] Value_1 = 1,
		}
		public override uint? ClassCRC => 0x276A5E3B;
	}
}

