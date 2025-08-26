namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_HunterHoleAIComponent_Template : AIComponent_Template {
		public float findEnemyRadius;
		public int useRadius;
		public AABB detectionRange;
		public Generic<TemplateAIBehavior> idleBehavior;
		public Generic<Ray_AIHunterAttackBehavior_Template> attackBehavior;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			findEnemyRadius = s.Serialize<float>(findEnemyRadius, name: "findEnemyRadius");
			useRadius = s.Serialize<int>(useRadius, name: "useRadius");
			detectionRange = s.SerializeObject<AABB>(detectionRange, name: "detectionRange");
			idleBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(idleBehavior, name: "idleBehavior");
			attackBehavior = s.SerializeObject<Generic<Ray_AIHunterAttackBehavior_Template>>(attackBehavior, name: "attackBehavior");
		}
		public override uint? ClassCRC => 0x6165B732;
	}
}

