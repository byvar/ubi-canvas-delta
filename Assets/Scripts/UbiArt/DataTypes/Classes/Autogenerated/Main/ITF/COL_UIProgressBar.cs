namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_UIProgressBar : UIComponent {
		public float barHeight;
		public float barWidth;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				barHeight = s.Serialize<float>(barHeight, name: "barHeight");
				barWidth = s.Serialize<float>(barWidth, name: "barWidth");
			}
		}
		public override uint? ClassCRC => 0x11D4CEB0;
	}
}

