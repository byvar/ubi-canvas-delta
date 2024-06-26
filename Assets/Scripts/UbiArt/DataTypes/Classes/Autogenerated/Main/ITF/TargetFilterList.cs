namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.VH | GameFlags.RA)]
	public partial class TargetFilterList : CSerializable {
		public Platform platform;
		public CArrayP<string> objects;
		public CString platformStr;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion == EngineVersion.RO) {
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					platformStr = s.Serialize<CString>(platformStr, name: "platformStr");
					objects = s.SerializeObject<CArrayP<string>>(objects, name: "objects");
				}
			} else {
				platform = s.SerializeObject<Platform>(platform, name: "platform");
				objects = s.SerializeObject<CArrayP<string>>(objects, name: "objects");
			}
		}
	}
}

