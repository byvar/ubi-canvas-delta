namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DarktoonTrapAIComponent_Template : Ray_AIComponent_Template {
		public Path holePath;
		public Vec3d holeOffset;
		public Generic<TemplateAIBehavior> trapBehavior;
		public Generic<TemplateAIBehavior> receiveHitBehavior;
		public Generic<TemplateAIBehavior> deathBehavior;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			holePath = s.SerializeObject<Path>(holePath, name: "holePath");
			holeOffset = s.SerializeObject<Vec3d>(holeOffset, name: "holeOffset");
			trapBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(trapBehavior, name: "trapBehavior");
			receiveHitBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(receiveHitBehavior, name: "receiveHitBehavior");
			deathBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(deathBehavior, name: "deathBehavior");
		}
		public override uint? ClassCRC => 0xD72D2A85;
	}
}

