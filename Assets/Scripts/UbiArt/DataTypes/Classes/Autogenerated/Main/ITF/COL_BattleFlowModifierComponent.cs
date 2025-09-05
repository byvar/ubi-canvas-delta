namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleFlowModifierComponent : ActorComponent {
		public COL_BattleFlowModifierComponent.EndOfBattleData EndOfBattle;
		public StringID IntroCinematicID;
		public StringID OutroWinCinematicID;
		public StringID OutroLossCinematicID;
		public int SynchronousIntro;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			EndOfBattle = s.SerializeObject<COL_BattleFlowModifierComponent.EndOfBattleData>(EndOfBattle, name: "EndOfBattle");
			IntroCinematicID = s.SerializeObject<StringID>(IntroCinematicID, name: "IntroCinematicID");
			OutroWinCinematicID = s.SerializeObject<StringID>(OutroWinCinematicID, name: "OutroWinCinematicID");
			OutroLossCinematicID = s.SerializeObject<StringID>(OutroLossCinematicID, name: "OutroLossCinematicID");
			SynchronousIntro = s.Serialize<int>(SynchronousIntro, name: "SynchronousIntro");
		}
		public override uint? ClassCRC => 0xF4D8C124;


		[Games(GameFlags.COL)]
		public class EndOfBattleData : CSerializable {
			public Path movie_path;
			public Path map_path;
			public Path map_path_TRIAL;
			public StringID mapLocationId;
			public StringID fleeMapLocationId;
			public StringID lossMapLocationId;
			public bool doVictoryFlow;
			public bool doLossFlow;
			public bool doLevelUpFlow;
			public bool waitForDeathAnim;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				movie_path = s.SerializeObject<Path>(movie_path, name: "movie_path");
				map_path = s.SerializeObject<Path>(map_path, name: "map_path");
				map_path_TRIAL = s.SerializeObject<Path>(map_path_TRIAL, name: "map_path_TRIAL");
				mapLocationId = s.SerializeObject<StringID>(mapLocationId, name: "mapLocationId");
				fleeMapLocationId = s.SerializeObject<StringID>(fleeMapLocationId, name: "fleeMapLocationId");
				lossMapLocationId = s.SerializeObject<StringID>(lossMapLocationId, name: "lossMapLocationId");
				doVictoryFlow = s.Serialize<bool>(doVictoryFlow, name: "doVictoryFlow", options: CSerializerObject.Options.BoolAsByte);
				doLossFlow = s.Serialize<bool>(doLossFlow, name: "doLossFlow", options: CSerializerObject.Options.BoolAsByte);
				doLevelUpFlow = s.Serialize<bool>(doLevelUpFlow, name: "doLevelUpFlow", options: CSerializerObject.Options.BoolAsByte);
				waitForDeathAnim = s.Serialize<bool>(waitForDeathAnim, name: "waitForDeathAnim", options: CSerializerObject.Options.BoolAsByte);
			}
		}
	}
}

