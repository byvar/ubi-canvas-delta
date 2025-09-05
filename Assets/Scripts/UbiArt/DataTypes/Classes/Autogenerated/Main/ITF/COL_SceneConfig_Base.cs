namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_SceneConfig_Base : SceneConfig {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA61CE89C;
	}
}

