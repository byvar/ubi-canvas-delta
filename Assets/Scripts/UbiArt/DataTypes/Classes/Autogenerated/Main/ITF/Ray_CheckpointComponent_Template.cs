namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CheckpointComponent_Template : CheckpointComponent_Template {
		public Path Sequence;
		public int commitGameState;
		public float waitTime;
		public float timeBetweenRank;
		public float displayScoreTime;
		public CListO<StringID> startAnimations;
		public CListO<StringID> animationsByRank;
		public CListO<StringID> boneNamesByRank;
		public CListO<Path> fakePlayers;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			Sequence = s.SerializeObject<Path>(Sequence, name: "Sequence");
			commitGameState = s.Serialize<int>(commitGameState, name: "commitGameState");
			waitTime = s.Serialize<float>(waitTime, name: "waitTime");
			timeBetweenRank = s.Serialize<float>(timeBetweenRank, name: "timeBetweenRank");
			displayScoreTime = s.Serialize<float>(displayScoreTime, name: "displayScoreTime");
			startAnimations = s.SerializeObject<CListO<StringID>>(startAnimations, name: "startAnimations");
			animationsByRank = s.SerializeObject<CListO<StringID>>(animationsByRank, name: "animationsByRank");
			boneNamesByRank = s.SerializeObject<CListO<StringID>>(boneNamesByRank, name: "boneNamesByRank");
			fakePlayers = s.SerializeObject<CListO<Path>>(fakePlayers, name: "fakePlayers");
		}
		public override uint? ClassCRC => 0x4C452AEC;
	}
}

