namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CinematicDialogsUIComponent_Template : UIComponent_Template {
		public StringID characterID;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			characterID = s.SerializeObject<StringID>(characterID, name: "characterID");
		}
		public override uint? ClassCRC => 0xE30B7630;
	}
}

