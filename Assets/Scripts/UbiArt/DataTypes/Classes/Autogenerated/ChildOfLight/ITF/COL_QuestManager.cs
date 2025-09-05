namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_QuestManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8F96C5EE;
	}
}

