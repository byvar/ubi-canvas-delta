namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion)]
	public partial class MusicTreeNodePlayMusic : MusicTreeNode {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x44124AFE;
	}
}

