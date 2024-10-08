namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_UIItemDRCComponent : UIItem {
		public uint highlightTextStyle;
		public Vec3d buttonIconOffset;
		public Vec3d newContentOffset;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			highlightTextStyle = s.Serialize<uint>(highlightTextStyle, name: "highlightTextStyle");
			buttonIconOffset = s.SerializeObject<Vec3d>(buttonIconOffset, name: "buttonIconOffset");
			newContentOffset = s.SerializeObject<Vec3d>(newContentOffset, name: "newContentOffset");
		}
		public override uint? ClassCRC => 0x049FE29A;
	}
}

