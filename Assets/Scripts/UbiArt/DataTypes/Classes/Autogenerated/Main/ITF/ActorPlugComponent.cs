namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class ActorPlugComponent : ActorComponent {
		public CArrayO<Generic<ActorPlugBaseController>> controllers;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.COL) {
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					controllers = s.SerializeObject<CArrayO<Generic<ActorPlugBaseController>>>(controllers, name: "controllers");
				}
			}
		}
		public override uint? ClassCRC => 0x6616AC1F;
	}
}

