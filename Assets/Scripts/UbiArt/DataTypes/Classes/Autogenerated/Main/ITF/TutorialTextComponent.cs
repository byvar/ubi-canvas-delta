namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class TutorialTextComponent : ActorComponent {
		public Vec2d RELATIVEPOS = new Vec2d(0.5f, 0.5f);
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				RELATIVEPOS = s.SerializeObject<Vec2d>(RELATIVEPOS, name: "RELATIVEPOS");
			}
		}
		public override uint? ClassCRC => 0x92A7EA4E;
	}
}

