namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_WorldMapMenu : UIMenuBasic {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x203D4AD8;
	}
}

