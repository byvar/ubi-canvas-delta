namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_ObjectSaveData : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC329424D;
	}
}

