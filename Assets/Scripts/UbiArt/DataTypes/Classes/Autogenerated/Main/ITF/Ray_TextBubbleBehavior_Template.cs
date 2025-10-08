namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_TextBubbleBehavior_Template : TemplateAIBehavior {
		public StringID textBoneName;
		public Generic<AIAction_Template> sleepAction;
		public Generic<AIAction_Template> openAction;
		public Generic<AIAction_Template> idleAction;
		public Generic<AIAction_Template> closeAction;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			textBoneName = s.SerializeObject<StringID>(textBoneName, name: "textBoneName");
			sleepAction = s.SerializeObject<Generic<AIAction_Template>>(sleepAction, name: "sleepAction");
			openAction = s.SerializeObject<Generic<AIAction_Template>>(openAction, name: "openAction");
			idleAction = s.SerializeObject<Generic<AIAction_Template>>(idleAction, name: "idleAction");
			closeAction = s.SerializeObject<Generic<AIAction_Template>>(closeAction, name: "closeAction");
		}
		public override uint? ClassCRC => 0x11E52943;
	}
}

