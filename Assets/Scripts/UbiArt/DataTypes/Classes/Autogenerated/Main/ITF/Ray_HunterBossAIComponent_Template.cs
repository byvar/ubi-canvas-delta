namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_HunterBossAIComponent_Template : Ray_AIComponent_Template {
		public Generic<TemplateAIBehavior> idleBehavior;
		public Generic<Ray_AIHunterAttackBehavior_Template> attackBehavior;
		public Generic<Ray_AIHunterAttackBehavior_Template> holeAttackBehavior;
		public Generic<Ray_AIReceiveHitBehavior_Template> receiveHitBehavior;
		public Placeholder deathBehavior;
		public Placeholder closeRangeBehavior;
		public Generic<TemplateAIBehavior> crushedBehavior;
		public Generic<TemplateAIBehavior> voidBehavior;
		public float findEnemyRadius;
		public float closeRadius;
		public int useRadius;
		public AABB detectionRange;
		public AABB detectionCloseRange;
		public uint healthBeforeChange;
		public uint maxNodes;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			idleBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(idleBehavior, name: "idleBehavior");
			attackBehavior = s.SerializeObject<Generic<Ray_AIHunterAttackBehavior_Template>>(attackBehavior, name: "attackBehavior");
			holeAttackBehavior = s.SerializeObject<Generic<Ray_AIHunterAttackBehavior_Template>>(holeAttackBehavior, name: "holeAttackBehavior");
			receiveHitBehavior = s.SerializeObject<Generic<Ray_AIReceiveHitBehavior_Template>>(receiveHitBehavior, name: "receiveHitBehavior");
			deathBehavior = s.SerializeObject<Placeholder>(deathBehavior, name: "deathBehavior");
			closeRangeBehavior = s.SerializeObject<Placeholder>(closeRangeBehavior, name: "closeRangeBehavior");
			crushedBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(crushedBehavior, name: "crushedBehavior");
			voidBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(voidBehavior, name: "voidBehavior");
			findEnemyRadius = s.Serialize<float>(findEnemyRadius, name: "findEnemyRadius");
			closeRadius = s.Serialize<float>(closeRadius, name: "closeRadius");
			useRadius = s.Serialize<int>(useRadius, name: "useRadius");
			detectionRange = s.SerializeObject<AABB>(detectionRange, name: "detectionRange");
			detectionCloseRange = s.SerializeObject<AABB>(detectionCloseRange, name: "detectionCloseRange");
			healthBeforeChange = s.Serialize<uint>(healthBeforeChange, name: "healthBeforeChange");
			maxNodes = s.Serialize<uint>(maxNodes, name: "maxNodes");
		}
		public override uint? ClassCRC => 0xDFA8946A;
	}
}

