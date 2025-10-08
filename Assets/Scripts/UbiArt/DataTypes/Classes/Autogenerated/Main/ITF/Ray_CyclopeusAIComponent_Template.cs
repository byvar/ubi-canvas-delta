namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CyclopeusAIComponent_Template : Ray_SimpleAIComponent_Template {
		public Generic<TemplateAIBehavior> hurtBehavior;
		public uint hitMax;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			hurtBehavior = s.SerializeObject<Generic<TemplateAIBehavior>>(hurtBehavior, name: "hurtBehavior");
			hitMax = s.Serialize<uint>(hitMax, name: "hitMax");
		}
		public override uint? ClassCRC => 0xBF9B3E0C;
	}
}

