namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RAVersion)]
	public partial class RO2_BezierTreeComponent_Template : ActorComponent_Template {
		public RO2_BezierTree_Template tree = new RO2_BezierTree_Template();
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			// Not serialized as an object with a group, just raw!
			tree.SerializeEmbedded(s);
		}
		public override uint? ClassCRC => 0x23D04434;
	}
}

