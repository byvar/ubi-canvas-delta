namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class MusicTreeBlock : MusicTreeNode {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE24EBF49;
	}
}

