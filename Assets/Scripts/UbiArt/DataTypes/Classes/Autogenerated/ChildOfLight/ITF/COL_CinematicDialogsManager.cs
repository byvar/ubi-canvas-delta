namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CinematicDialogsManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5D43CD2A;
	}
}

