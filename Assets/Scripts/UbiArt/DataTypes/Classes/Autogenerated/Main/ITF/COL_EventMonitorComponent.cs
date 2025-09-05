namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_EventMonitorComponent : ActorComponent {
		public uint numToMonitor;
		public Generic<Event> successEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			numToMonitor = s.Serialize<uint>(numToMonitor, name: "numToMonitor");
			successEvent = s.SerializeObject<Generic<Event>>(successEvent, name: "successEvent");
		}
		public override uint? ClassCRC => 0xA821D824;
	}
}

