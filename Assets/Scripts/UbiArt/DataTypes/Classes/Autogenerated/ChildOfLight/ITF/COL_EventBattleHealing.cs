namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_EventBattleHealing : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC5D298FC;
	}
}

