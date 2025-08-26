namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_StoneManAIComponent_Template : Ray_GroundEnemyAIComponent_Template {
		public Generic<Ray_AIReceiveHitBehavior_Template> receiveHitWithBoulderBehaviorNew;
		public Generic<TemplateAIBehavior> closeRangeAttackWithBoulderBehavior;
		public Generic<TemplateAIBehavior> closeRangeBackAttackBehavior;
		public Generic<TemplateAIBehavior> closeRangeBackAttackWithBoulderBehavior;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			receiveHitWithBoulderBehaviorNew = s.SerializeObject<Generic<Ray_AIReceiveHitBehavior_Template>>(receiveHitWithBoulderBehaviorNew, name: "receiveHitWithBoulderBehaviorNew");
			closeRangeAttackWithBoulderBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(closeRangeAttackWithBoulderBehavior, name: "closeRangeAttackWithBoulderBehavior");
			closeRangeBackAttackBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(closeRangeBackAttackBehavior, name: "closeRangeBackAttackBehavior");
			closeRangeBackAttackWithBoulderBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(closeRangeBackAttackWithBoulderBehavior, name: "closeRangeBackAttackWithBoulderBehavior");
		}
		public override uint? ClassCRC => 0xB995CB03;
	}
}

