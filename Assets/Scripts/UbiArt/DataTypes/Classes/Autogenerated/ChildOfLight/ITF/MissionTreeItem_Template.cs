namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionTreeItem_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3ADB61E8;
	}
}

