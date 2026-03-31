namespace UbiArt.Localisation {
	// See: ITF::LocAudio::serialize
	public class LocAudio : CSerializable {
		public uint fileIndex;
		public string filename;
		public float volume;

		public CString filenameOrigins {
			get => filename;
			set => filename = value;
		}

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			fileIndex = s.Serialize<uint>(fileIndex, name: "fileIndex");
			if (s.Settings.EngineVersion == EngineVersion.RO) {
				filenameOrigins = s.Serialize<CString>(filenameOrigins, name: "filename");
			} else {
				filename = s.Serialize<string>(filename, name: "filename");
			}
			volume = s.Serialize<float>(volume, name: "volume");
		}
	}
}
