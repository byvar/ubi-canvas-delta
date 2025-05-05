namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_ScoreRecapManagerComponent_Template : ActorComponent_Template {
		public float timeBetweenLums;
		public float timeAppearNewPet;
		public StringID animPlayerDance;
		public Path petPath;
		public uint maxConfetti;
		public float prisonersMinDelayShort;
		public float prisonersMaxDelayShort;
		public float prisonersMinDelayLong;
		public float prisonersMaxDelayLong;
		public Generic<Event> musicEventPrisonersStart;
		public Generic<Event> musicEventLowEnd;
		public Generic<Event> musicEventHighEnd;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				timeBetweenLums = s.Serialize<float>(timeBetweenLums, name: "timeBetweenLums");
				timeAppearNewPet = s.Serialize<float>(timeAppearNewPet, name: "timeAppearNewPet");
				animPlayerDance = s.SerializeObject<StringID>(animPlayerDance, name: "animPlayerDance");
				petPath = s.SerializeObject<Path>(petPath, name: "petPath");
				maxConfetti = s.Serialize<uint>(maxConfetti, name: "maxConfetti");
				prisonersMinDelayShort = s.Serialize<float>(prisonersMinDelayShort, name: "prisonersMinDelayShort");
				prisonersMaxDelayShort = s.Serialize<float>(prisonersMaxDelayShort, name: "prisonersMaxDelayShort");
				prisonersMinDelayLong = s.Serialize<float>(prisonersMinDelayLong, name: "prisonersMinDelayLong");
				prisonersMaxDelayLong = s.Serialize<float>(prisonersMaxDelayLong, name: "prisonersMaxDelayLong");
				musicEventPrisonersStart = s.SerializeObject<Generic<Event>>(musicEventPrisonersStart, name: "musicEventPrisonersStart");
				musicEventLowEnd = s.SerializeObject<Generic<Event>>(musicEventLowEnd, name: "musicEventLowEnd");
				musicEventHighEnd = s.SerializeObject<Generic<Event>>(musicEventHighEnd, name: "musicEventHighEnd");
			} else {
				timeBetweenLums = s.Serialize<float>(timeBetweenLums, name: "timeBetweenLums");
				timeAppearNewPet = s.Serialize<float>(timeAppearNewPet, name: "timeAppearNewPet");
				animPlayerDance = s.SerializeObject<StringID>(animPlayerDance, name: "animPlayerDance");
				petPath = s.SerializeObject<Path>(petPath, name: "petPath");
				maxConfetti = s.Serialize<uint>(maxConfetti, name: "maxConfetti");
				prisonersMinDelayShort = s.Serialize<float>(prisonersMinDelayShort, name: "prisonersMinDelayShort");
				prisonersMaxDelayShort = s.Serialize<float>(prisonersMaxDelayShort, name: "prisonersMaxDelayShort");
				prisonersMinDelayLong = s.Serialize<float>(prisonersMinDelayLong, name: "prisonersMinDelayLong");
				prisonersMaxDelayLong = s.Serialize<float>(prisonersMaxDelayLong, name: "prisonersMaxDelayLong");
			}
		}
		public override uint? ClassCRC => 0x9DA6639F;
	}
}

