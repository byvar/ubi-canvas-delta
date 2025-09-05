namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_EventStartCinematicDialog : Event {
		public COL_CinematicDialogData data;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			data = s.SerializeObject<COL_CinematicDialogData>(data, name: "data");
		}
		public override uint? ClassCRC => 0x533E6688;
	}
}

