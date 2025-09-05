namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PCState_Battle_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8528C8E6;
	}
}

