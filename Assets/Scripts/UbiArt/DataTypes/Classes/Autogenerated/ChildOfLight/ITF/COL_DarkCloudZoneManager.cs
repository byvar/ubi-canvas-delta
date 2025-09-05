namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_DarkCloudZoneManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x39E9E64F;
	}
}

