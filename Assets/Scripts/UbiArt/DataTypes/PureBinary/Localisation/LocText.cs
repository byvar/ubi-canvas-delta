namespace UbiArt.Localisation {
	// See: ITF::LocText::serialize
	public class LocText : CSerializable {
		public string text;

		public CString textOrigins {
			get => text;
			set => text = value;
		}
		public CString idOrigins;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion == EngineVersion.RO) {
				if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR) {
					idOrigins = s.Serialize<CString>(idOrigins, name: "id");
				}
				textOrigins = s.Serialize<CString>(textOrigins, name: "text");
			} else {
				text = s.Serialize<string>(text, name: "text", options: CSerializerObject.Options.UTF8);
			}
		}
	}
}
