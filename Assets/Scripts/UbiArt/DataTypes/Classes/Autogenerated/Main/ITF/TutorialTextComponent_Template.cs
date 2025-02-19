namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class TutorialTextComponent_Template : ActorComponent_Template {
		public Vec2d animSize = new Vec2d(300, 300);
		public StringID iconPoint = "NewFeature_Pos";
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animSize = s.SerializeObject<Vec2d>(animSize, name: "animSize");
			iconPoint = s.SerializeObject<StringID>(iconPoint, name: "iconPoint");
		}
		public override uint? ClassCRC => 0x170E87C7;
	}
}

