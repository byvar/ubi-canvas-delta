namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InteractComponent : COL_BaseInteractiveComponent {
		public Enum_triggerMode triggerMode;
		public bool softSaveOnTrigger;
		public int triggered;
		public Generic<Event> onInteractEvent;
		public Generic<Event> onEndInteractEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			triggerMode = s.Serialize<Enum_triggerMode>(triggerMode, name: "triggerMode");
			softSaveOnTrigger = s.Serialize<bool>(softSaveOnTrigger, name: "softSaveOnTrigger", options: CSerializerObject.Options.BoolAsByte);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				triggered = s.Serialize<int>(triggered, name: "triggered");
			}
			onInteractEvent = s.SerializeObject<Generic<Event>>(onInteractEvent, name: "onInteractEvent");
			onEndInteractEvent = s.SerializeObject<Generic<Event>>(onEndInteractEvent, name: "onEndInteractEvent");
		}
		public enum Enum_triggerMode {
			[Serialize("Value_1")] Value_1 = 1,
			[Serialize("Value_2")] Value_2 = 2,
			[Serialize("Value_3")] Value_3 = 3,
			[Serialize("Value_4")] Value_4 = 4,
		}
		public override uint? ClassCRC => 0x1E7C0399;
	}
}

