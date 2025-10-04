namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class PadRumble : CSerializable {
		public StringID name;
		public float intensity;
		public float lightIntensity;
		public float duration;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.VH || s.Settings.Game == Game.RL || s.Settings.Game == Game.RO || s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR) {
				name = s.SerializeObject<StringID>(name, name: "name");
				intensity = s.Serialize<float>(intensity, name: "intensity");
				duration = s.Serialize<float>(duration, name: "duration");
			} else {
				name = s.SerializeObject<StringID>(name, name: "name");
				intensity = s.Serialize<float>(intensity, name: "intensity");
				lightIntensity = s.Serialize<float>(lightIntensity, name: "lightIntensity");
				duration = s.Serialize<float>(duration, name: "duration");
			}
		}
	}
}

