namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AILivingStoneDrowningBehavior_Template : Ray_AIWaterBaseBehavior_Template {
		public Generic<AIAction_Template> dive;
		public Generic<AIAction_Template> drown;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			dive = s.SerializeObject<Generic<AIAction_Template>>(dive, name: "dive");
			drown = s.SerializeObject<Generic<AIAction_Template>>(drown, name: "drown");
		}
		public override uint? ClassCRC => 0x0D67DFC6;
	}
}

