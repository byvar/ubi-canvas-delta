namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DarkBirdAIComponent_Template : Ray_GroundEnemyAIComponent_Template {
		public Vec3d appear3dOffset;
		public int detachOnDeath;
		public Generic<TemplateAIBehavior> disappear3dBehavior;
		public Generic<TemplateAIBehavior> triggerBounceBehavior;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			appear3dOffset = s.SerializeObject<Vec3d>(appear3dOffset, name: "appear3dOffset");
			detachOnDeath = s.Serialize<int>(detachOnDeath, name: "detachOnDeath");
			disappear3dBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(disappear3dBehavior, name: "disappear3dBehavior");
			triggerBounceBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(triggerBounceBehavior, name: "triggerBounceBehavior");
		}
		public override uint? ClassCRC => 0x25D475DF;
	}
}

