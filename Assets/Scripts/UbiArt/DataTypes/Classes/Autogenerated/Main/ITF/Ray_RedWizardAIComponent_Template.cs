namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_RedWizardAIComponent_Template : Ray_GroundAIComponent_Template {
		public float followDetectDistance;
		public Generic<TemplateAIBehavior> followBehavior;
		public CListO<Generic<TemplateAIBehavior>> eventTriggeredBehaviors;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			followDetectDistance = s.Serialize<float>(followDetectDistance, name: "followDetectDistance");
			followBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(followBehavior, name: "followBehavior");
			eventTriggeredBehaviors = s.SerializeObject<CListO<Generic<TemplateAIBehavior>>>(eventTriggeredBehaviors, name: "eventTriggeredBehaviors");
		}
		public override uint? ClassCRC => 0x08D139D7;
	}
}

