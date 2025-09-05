namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BaseCharacterStatsManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x505F9E45;
	}
}

