namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIFriendly_JumpToTargetBehavior_Template : Ray_AIGroundBaseBehavior_Template {
		public Generic<AIAction_Template> jump;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			jump = s.SerializeObject<Generic<AIAction_Template>>(jump, name: "jump");
		}
		public override uint? ClassCRC => 0xF1567739;
	}
}

