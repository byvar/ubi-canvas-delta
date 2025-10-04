namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_HomeFriendComponent_Template : RO2_HomeComponent_Template {
		public Vec2d iconTauntAnchorOffset;
		public Vec2d iconLeaderAnchorOffset;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			iconTauntAnchorOffset = s.SerializeObject<Vec2d>(iconTauntAnchorOffset, name: "iconTauntAnchorOffset");
			iconLeaderAnchorOffset = s.SerializeObject<Vec2d>(iconLeaderAnchorOffset, name: "iconLeaderAnchorOffset");
		}
		public override uint? ClassCRC => 0xF16C419D;
	}
}

