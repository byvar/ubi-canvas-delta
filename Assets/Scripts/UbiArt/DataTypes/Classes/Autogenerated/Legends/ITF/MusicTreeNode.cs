namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class MusicTreeNode : BlendTreeNodeBlend<MusicTreeResult> {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7B7FC5BA;
	}
}

