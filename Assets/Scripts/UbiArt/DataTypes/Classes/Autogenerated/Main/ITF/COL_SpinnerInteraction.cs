namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_SpinnerInteraction : COL_ObjectInteraction {
		public uint numStates;
		public float animDuration;
		public uint initialState;
		public bool applyToClient;
		public bool applyToSelf;
		public StringID startFx;
		public StringID stopFx;
		public bool loop;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			numStates = s.Serialize<uint>(numStates, name: "numStates");
			animDuration = s.Serialize<float>(animDuration, name: "animDuration");
			initialState = s.Serialize<uint>(initialState, name: "initialState");
			applyToClient = s.Serialize<bool>(applyToClient, name: "applyToClient", options: CSerializerObject.Options.BoolAsByte);
			applyToSelf = s.Serialize<bool>(applyToSelf, name: "applyToSelf", options: CSerializerObject.Options.BoolAsByte);
			startFx = s.SerializeObject<StringID>(startFx, name: "startFx");
			stopFx = s.SerializeObject<StringID>(stopFx, name: "stopFx");
			loop = s.Serialize<bool>(loop, name: "loop", options: CSerializerObject.Options.BoolAsByte);
		}
		public override uint? ClassCRC => 0xCCC8C468;
	}
}

