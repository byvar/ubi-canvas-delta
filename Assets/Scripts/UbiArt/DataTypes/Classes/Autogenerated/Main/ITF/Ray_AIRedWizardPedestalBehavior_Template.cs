namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIRedWizardPedestalBehavior_Template : TemplateAIBehavior {
		public Generic<AIAction_Template> idle;
		public StringID supportBone;
		public Path gameMaterial;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			idle = s.SerializeObject<Generic<AIAction_Template>>(idle, name: "idle");
			supportBone = s.SerializeObject<StringID>(supportBone, name: "supportBone");
			gameMaterial = s.SerializeObject<Path>(gameMaterial, name: "gameMaterial");
		}
		public override uint? ClassCRC => 0x98A688C6;
	}
}

