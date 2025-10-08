namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIShooterCloseAttackBehavior_Template : TemplateAIBehavior {
		public Generic<AIAction_Template> attack;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			attack = s.SerializeObject<Generic<AIAction_Template>>(attack, name: "attack");
		}
		public override uint? ClassCRC => 0x57732622;
	}
}

