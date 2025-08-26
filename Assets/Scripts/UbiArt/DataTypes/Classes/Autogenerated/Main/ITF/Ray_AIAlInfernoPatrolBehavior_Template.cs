namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIAlInfernoPatrolBehavior_Template : Ray_AIGroundBaseMovementBehavior_Template {
		public Generic<AIAction_Template> fall;
		public Generic<AIAction_Template> defaultPause;
		public Generic<AIAction_Template> attack;
		public float uturnDelay2;
		public AABB detectionRange;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			fall = s.SerializeObject<Generic<AIAction_Template>>(fall, name: "fall");
			defaultPause = s.SerializeObject<Generic<AIAction_Template>>(defaultPause, name: "defaultPause");
			attack = s.SerializeObject<Generic<AIAction_Template>>(attack, name: "attack");
			uturnDelay2 = s.Serialize<float>(uturnDelay2, name: "uturnDelay2");
			detectionRange = s.SerializeObject<AABB>(detectionRange, name: "detectionRange");
		}
		public override uint? ClassCRC => 0x2CE9AE3F;
	}
}

