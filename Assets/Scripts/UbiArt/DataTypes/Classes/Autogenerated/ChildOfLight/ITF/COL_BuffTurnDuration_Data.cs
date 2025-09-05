namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffTurnDuration_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB8441C78;
	}
}

