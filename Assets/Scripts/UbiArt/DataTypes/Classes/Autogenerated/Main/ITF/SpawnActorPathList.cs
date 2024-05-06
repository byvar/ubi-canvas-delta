namespace UbiArt.ITF {
	[Games(GameFlags.RA)]
	public partial class SpawnActorPathList : CSerializable {
		public CListO<SpawnActorPathTuto> tutoList;

		// Vita hack
		public Path[] TutorialsVita0 { get; set; } = new Path[106];
		public CListO<Path> difficultyTutorials;
		public Path[] TutorialsVita1 { get; set; } = new Path[48];
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Platform == GamePlatform.Vita) {
				for (int i = 0; i < TutorialsVita0.Length; i++) {
					TutorialsVita0[i] = s.SerializeObject<Path>(TutorialsVita0[i], name: $"{nameof(TutorialsVita0)}[{i}]");
				}
				difficultyTutorials = s.SerializeObject<CListO<Path>>(difficultyTutorials, name: "difficultyTutorials");
				for (int i = 0; i < TutorialsVita1.Length; i++) {
					TutorialsVita1[i] = s.SerializeObject<Path>(TutorialsVita1[i], name: $"{nameof(TutorialsVita1)}[{i}]");
				}
			} else {
				tutoList = s.SerializeObject<CListO<SpawnActorPathTuto>>(tutoList, name: "tutoList");
			}
		}
	}
}

