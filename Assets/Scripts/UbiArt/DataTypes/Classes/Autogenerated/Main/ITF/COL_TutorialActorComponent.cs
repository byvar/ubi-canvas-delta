namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TutorialActorComponent : COL_BaseTutorialActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x87C26FD6;
	}
}

