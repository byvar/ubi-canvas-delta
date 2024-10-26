namespace UbiArt.ITF {
	public static class EventExtensions {
		public static bool IsAdventuresExclusive(this Event e) {
			if (e != null) {
				if (e.GetType().Name.Contains("RLC_")) return true;

				switch (e) {
					case EventLoadWwiseBank:
					case EventPlayWwiseEvent:
					case EventResetWwiseAuxBusEffect:
					case EventSetWwiseAuxBusEffect:
					case EventSetWwiseState:
					case EventSetWwiseSwitch:
					case EventUnloadWwiseBank:
						return true;
				}
			}
			return false;
		}

		public static Event GetWorkaroundEvent(this Event e) {
			//Event newE;
			if (e != null) {
				switch (e) {
					case EventGeneric gen:
						// We have to misuse a few different events as RelayEventComponent doesn't check
						// the ID of the generic event in Legends and relays it regardless...
						switch (gen.id?.stringID ?? uint.MaxValue) {
							case 0xA4EAA5D3: return new RO2_EventDragonSwitchAnimation();
							case 0x6360213D: return new RO2_EventDragonSwitchPhase();
							case 0xDDD92049: return new RO2_EventDragonUnPause();
						}
						break;
					/*case EventBreakableBreak ebb:
						newE = Merger.Merge<RO2_EventBreakableBreak>(ebb);
						return newE;
					case EventBreakableQuery ebq:
						newE = Merger.Merge<RO2_EventBreakableQuery>(ebq);
						return newE;*/
				}
			}
			return null;
		}
	}
}
