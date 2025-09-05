namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PauseMenu_Template : UIMenuBasic_Template {
		public float skillPtsCountOffsetX;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			skillPtsCountOffsetX = s.Serialize<float>(skillPtsCountOffsetX, name: "skillPtsCountOffsetX");
		}
		public override uint? ClassCRC => 0x0A7A2507;
	}
}

