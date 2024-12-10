namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_TalkingHatAIComponent : ActorComponent {
		public CListO<LocalisationId> sentences;
		public int vitaProperty;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				sentences = s.SerializeObject<CListO<LocalisationId>>(sentences, name: "sentences");
				if (s.Settings.Platform == GamePlatform.Vita) {
					vitaProperty = s.Serialize<int>(vitaProperty, name: nameof(vitaProperty));
				}
			}
		}
		public override uint? ClassCRC => 0xCD01898F;
	}
}

