namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIDarktoonTrapBehavior_Template : TemplateAIBehavior {
		public int startsHidden;
		public float heightFactor;
		public float attackRate;
		public float coolDownDuration;
		public uint returnHitMaxLevel;
		public Generic<PhysShape> outerShape;
		public Generic<PhysShape> innerShape;
		public Generic<AIAction_Template> idle;
		public Generic<AIAction_Template> attack;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			startsHidden = s.Serialize<int>(startsHidden, name: "startsHidden");
			heightFactor = s.Serialize<float>(heightFactor, name: "heightFactor");
			attackRate = s.Serialize<float>(attackRate, name: "attackRate");
			coolDownDuration = s.Serialize<float>(coolDownDuration, name: "coolDownDuration");
			returnHitMaxLevel = s.Serialize<uint>(returnHitMaxLevel, name: "returnHitMaxLevel");
			outerShape = s.SerializeObject<Generic<PhysShape>>(outerShape, name: "outerShape");
			innerShape = s.SerializeObject<Generic<PhysShape>>(innerShape, name: "innerShape");
			idle = s.SerializeObject<Generic<AIAction_Template>>(idle, name: "idle");
			attack = s.SerializeObject<Generic<AIAction_Template>>(attack, name: "attack");
		}
		public override uint? ClassCRC => 0xED2F49D6;
	}
}

