namespace UbiArt.ITF {
	[Games(GameFlags.RM)]
	public partial class RLC_ChallengeDefinition : CSerializable {
		public StringID WorldId;
		public LocalisationId WorldLocId;
		public Path MapPath;
		public Path BackgroundPath;
		public CListO<Path> IsgPaths;
		public CListO<Path> IsgPathsMrDark;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			WorldId = s.SerializeObject<StringID>(WorldId, name: "WorldId");
			WorldLocId = s.SerializeObject<LocalisationId>(WorldLocId, name: "WorldLocId");
			MapPath = s.SerializeObject<Path>(MapPath, name: "MapPath");
			BackgroundPath = s.SerializeObject<Path>(BackgroundPath, name: "BackgroundPath");
			IsgPaths = s.SerializeObject<CListO<Path>>(IsgPaths, name: "IsgPaths");
			IsgPathsMrDark = s.SerializeObject<CListO<Path>>(IsgPathsMrDark, name: "IsgPathsMrDark");
		}
		public override uint? ClassCRC => 0x4BC3E9A3;
	}
}

