namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RAVersion)]
	public partial class AIBounceToLayerAction_Template : AIAction_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF1BE0710;
	}
}

