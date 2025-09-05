namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PCState_Death_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xCC06F43E;
	}
}

