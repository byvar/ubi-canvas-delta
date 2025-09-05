namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_FlyingMeter_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC3A2EB2A;
	}
}

