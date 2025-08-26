namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIStoneManAttackBehavior_Template : TemplateAIBehavior {
		public Generic<AIAction_Template> respawnBoulder;
		public Generic<AIAction_Template> idle;
		public Generic<AIAction_Template> anticipation;
		public Generic<AIAction_Template> sleep;
		public Generic<AIAction_Template> wakeUp;
		public CListO<Generic<AIAction_Template>> launchActions;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			respawnBoulder = s.SerializeObject<Generic<AIAction_Template>>(respawnBoulder, name: "respawnBoulder");
			idle = s.SerializeObject<Generic<AIAction_Template>>(idle, name: "idle");
			anticipation = s.SerializeObject<Generic<AIAction_Template>>(anticipation, name: "anticipation");
			sleep = s.SerializeObject<Generic<AIAction_Template>>(sleep, name: "sleep");
			wakeUp = s.SerializeObject<Generic<AIAction_Template>>(wakeUp, name: "wakeUp");
			launchActions = s.SerializeObject<CListO<Generic<AIAction_Template>>>(launchActions, name: "launchActions");
		}
		public override uint? ClassCRC => 0x8DE04F71;
	}
}

