namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIBounceRoamingBehavior_Template : TemplateAIBehavior {
		public Generic<AIJumpInDirAction_Template> bounceMove;
		public Generic<AIJumpInDirAction_Template> bounceIdle;
		public Generic<AIJumpAction_Template> bounceJump;
		public float minTimeToWalk;
		public float maxTimeToWalk;
		public float minTimeToIdle;
		public float maxTimeToIdle;
		public float obstacleDetectionDistance;
		public float wallJumpHeight;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			bounceMove = s.SerializeObject<Generic<AIJumpInDirAction_Template>>(bounceMove, name: "bounceMove");
			bounceIdle = s.SerializeObject<Generic<AIJumpInDirAction_Template>>(bounceIdle, name: "bounceIdle");
			bounceJump = s.SerializeObject<Generic<AIJumpAction_Template>>(bounceJump, name: "bounceJump");
			minTimeToWalk = s.Serialize<float>(minTimeToWalk, name: "minTimeToWalk");
			maxTimeToWalk = s.Serialize<float>(maxTimeToWalk, name: "maxTimeToWalk");
			minTimeToIdle = s.Serialize<float>(minTimeToIdle, name: "minTimeToIdle");
			maxTimeToIdle = s.Serialize<float>(maxTimeToIdle, name: "maxTimeToIdle");
			obstacleDetectionDistance = s.Serialize<float>(obstacleDetectionDistance, name: "obstacleDetectionDistance");
			wallJumpHeight = s.Serialize<float>(wallJumpHeight, name: "wallJumpHeight");
		}
		public override uint? ClassCRC => 0x3BB502ED;
	}
}

