namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_SceneConfig_InGame_Battle : COL_SceneConfig_Base {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC54E8665;
	}
}

