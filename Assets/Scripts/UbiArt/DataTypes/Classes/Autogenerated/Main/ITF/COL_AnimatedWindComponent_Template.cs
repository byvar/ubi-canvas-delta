namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_AnimatedWindComponent_Template : WindComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF5C4CC9B;
	}
}

