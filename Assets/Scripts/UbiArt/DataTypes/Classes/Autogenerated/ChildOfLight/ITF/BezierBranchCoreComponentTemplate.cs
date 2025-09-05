namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class BezierBranchCoreComponentTemplate : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x72F54A6B;
	}
}

