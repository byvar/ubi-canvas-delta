namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x73FF8BC0;
	}
}

