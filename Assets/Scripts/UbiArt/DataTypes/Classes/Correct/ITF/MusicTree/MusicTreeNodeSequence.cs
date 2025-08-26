namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion)]
	public partial class MusicTreeNodeSequence : MusicTreeNode {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBDDE5D01;
	}
}

