namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PCState_CoopFlying_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE4B9B3D8;
	}
}

