namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MissionPopupInteraction : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x984A8400;
	}
}

