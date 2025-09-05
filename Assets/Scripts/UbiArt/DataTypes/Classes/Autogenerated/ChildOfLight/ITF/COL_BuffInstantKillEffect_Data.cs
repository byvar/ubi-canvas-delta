namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BuffInstantKillEffect_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC590644A;
	}
}

