namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class BezierBranchCoreComponent : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBFBEB701;
	}
}

