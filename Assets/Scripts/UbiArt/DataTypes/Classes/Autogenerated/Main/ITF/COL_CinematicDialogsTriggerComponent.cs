namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CinematicDialogsTriggerComponent : ActorComponent {
		public COL_CinematicDialogData dialogData;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			dialogData = s.SerializeObject<COL_CinematicDialogData>(dialogData, name: "dialogData");
		}
		public override uint? ClassCRC => 0x475B791D;
	}
}

