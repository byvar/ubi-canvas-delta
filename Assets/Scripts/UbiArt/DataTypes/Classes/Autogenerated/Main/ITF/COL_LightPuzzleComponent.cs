namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightPuzzleComponent : ActorComponent {
		public bool drawDebug;
		public bool ordered;
		public float failCooldown;
		public float inRangeCoolDown;
		public Generic<Event> failEvent;
		public int isSolved;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			drawDebug = s.Serialize<bool>(drawDebug, name: "drawDebug", options: CSerializerObject.Options.BoolAsByte);
			ordered = s.Serialize<bool>(ordered, name: "ordered", options: CSerializerObject.Options.BoolAsByte);
			failCooldown = s.Serialize<float>(failCooldown, name: "failCooldown");
			inRangeCoolDown = s.Serialize<float>(inRangeCoolDown, name: "inRangeCoolDown");
			failEvent = s.SerializeObject<Generic<Event>>(failEvent, name: "failEvent");
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				isSolved = s.Serialize<int>(isSolved, name: "isSolved");
			}
		}
		public override uint? ClassCRC => 0x1D019BC4;
	}
}

