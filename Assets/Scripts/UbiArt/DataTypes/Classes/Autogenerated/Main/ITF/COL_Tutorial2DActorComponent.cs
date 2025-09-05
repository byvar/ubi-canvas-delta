namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_Tutorial2DActorComponent : COL_BaseTutorialActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x44CE0834;
	}
}

