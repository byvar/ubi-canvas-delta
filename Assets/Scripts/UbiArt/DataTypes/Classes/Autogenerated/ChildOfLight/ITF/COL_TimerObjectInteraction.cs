namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TimerObjectInteraction : COL_ObjectInteraction {
		public float inputtime;
		public float time;
		public StringID soundInput;
		public bool canPlayFx;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			inputtime = s.Serialize<float>(inputtime, name: "inputtime");
			time = s.Serialize<float>(time, name: "time");
			soundInput = s.SerializeObject<StringID>(soundInput, name: "soundInput");
			canPlayFx = s.Serialize<bool>(canPlayFx, name: "canPlayFx", options: CSerializerObject.Options.BoolAsByte);
		}
		public override uint? ClassCRC => 0x03ECC5B1;
	}
}

