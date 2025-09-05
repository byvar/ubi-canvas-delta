namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class SmartLocId : CSerializable {
		public string defaultText;
		public LocalisationId locId;
		public bool useText;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			defaultText = s.Serialize<string>(defaultText, name: "defaultText");
			locId = s.SerializeObject<LocalisationId>(locId, name: "locId");
			if (s.Settings.Game != Game.COL) {
				useText = s.Serialize<bool>(useText, name: "useText");
			}
		}
	}
}

