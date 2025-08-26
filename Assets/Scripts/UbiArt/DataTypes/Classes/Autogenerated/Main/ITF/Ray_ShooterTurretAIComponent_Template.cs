namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_ShooterTurretAIComponent_Template : Ray_MultiPiecesActorAIComponent_Template {
		public Generic<TemplateAIBehavior> idleBehavior;
		public Generic<Ray_AIShooterAttackBehavior_Template> attackBehavior;
		public Generic<Ray_AIReceiveHitBehavior_Template> receiveHitBehavior;
		public Generic<TemplateAIBehavior> deathBehavior;
		public Generic<TemplateAIBehavior> crushedBehavior;
		public Generic<TemplateAIBehavior> activateBehavior;
		public Generic<TemplateAIBehavior> desactivateBehavior;
		public Generic<TemplateAIBehavior> closedBehavior;
		public float findEnemyRadius;
		public float closeRadius;
		public int useRadius;
		public AABB detectionRange;
		public AABB detectionCloseRange;
		public int triggerable;
		public int forceLookDir;
		public int forceLookDirRight;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			idleBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(idleBehavior, name: "idleBehavior");
			attackBehavior = s.SerializeObject<Generic<Ray_AIShooterAttackBehavior_Template>>(attackBehavior, name: "attackBehavior");
			receiveHitBehavior = s.SerializeObject<Generic<Ray_AIReceiveHitBehavior_Template>>(receiveHitBehavior, name: "receiveHitBehavior");
			deathBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(deathBehavior, name: "deathBehavior");
			crushedBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(crushedBehavior, name: "crushedBehavior");
			activateBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(activateBehavior, name: "activateBehavior");
			desactivateBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(desactivateBehavior, name: "desactivateBehavior");
			closedBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(closedBehavior, name: "closedBehavior");
			findEnemyRadius = s.Serialize<float>(findEnemyRadius, name: "findEnemyRadius");
			closeRadius = s.Serialize<float>(closeRadius, name: "closeRadius");
			useRadius = s.Serialize<int>(useRadius, name: "useRadius");
			detectionRange = s.SerializeObject<AABB>(detectionRange, name: "detectionRange");
			detectionCloseRange = s.SerializeObject<AABB>(detectionCloseRange, name: "detectionCloseRange");
			triggerable = s.Serialize<int>(triggerable, name: "triggerable");
			forceLookDir = s.Serialize<int>(forceLookDir, name: "forceLookDir");
			forceLookDirRight = s.Serialize<int>(forceLookDirRight, name: "forceLookDirRight");
		}
		public override uint? ClassCRC => 0xCA358020;
	}
}

