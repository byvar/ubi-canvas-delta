namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class AnimTreeNodeBranchTransition_Template : BlendTreeNodeChooseBranch_Template<AnimTreeResult> {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x300D6F20;
	}
}

