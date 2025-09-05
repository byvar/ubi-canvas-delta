namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleSetupCharacterConfig_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xDF98EC7F;
	}
}

