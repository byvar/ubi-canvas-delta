namespace UbiArt.online {
	public partial class TimeInterval : CSerializable {
		public ulong value;
		public uint day;
		public uint hour;
		public uint minute;
		public uint second;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasProperties(SerializerProperties.Binary)) {
				value = s.Serialize<ulong>(value, name: "value");
			} else {
				if (s.HasFlags(SerializeFlags.Deprecate)) {
					value = s.Serialize<ulong>(value, name: "value");
				}
				day = s.Serialize<uint>(day, name: "day");
				hour = s.Serialize<uint>(hour, name: "hour");
				minute = s.Serialize<uint>(minute, name: "minute");
				second = s.Serialize<uint>(second, name: "second");
			}
		}
	}
}

