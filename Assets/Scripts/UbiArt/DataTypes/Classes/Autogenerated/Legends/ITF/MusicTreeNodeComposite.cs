namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class MusicTreeNodeComposite : MusicTreeNode {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x92E8BB21;
	}
}

