namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_UplayRewardsManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFAB3E7B4;
	}
}

