namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TutorialActorComponent_Template : COL_BaseTutorialActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xAD2AE824;
	}
}

