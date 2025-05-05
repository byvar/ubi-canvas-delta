namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class AnimNodeSwing : BlendTreeNodeChooseBranch<AnimTreeResult> {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x73BAE590;
	}
}

