namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class RelayEventInstanceComponent : ActorComponent {
		public CListO<RelayEventInstanceComponent.COL_RelayData> relays;
		public Generic<Event> resetEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			relays = s.SerializeObject<CListO<RelayEventInstanceComponent.COL_RelayData>>(relays, name: "relays");
			resetEvent = s.SerializeObject<Generic<Event>>(resetEvent, name: "resetEvent");
		}
		public override uint? ClassCRC => 0x02D88068;

		[Games(GameFlags.COL)]
		public class COL_RelayData : CSerializable {
			public Generic<Event> eventToListen;
			public EventSender eventToRelay;
			public float delay;
			public bool replaceSender;
			public bool replaceSenderByActivator;
			public bool resetTimerOnRetrigger;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				eventToListen = s.SerializeObject<Generic<Event>>(eventToListen, name: "eventToListen");
				eventToRelay = s.SerializeObject<EventSender>(eventToRelay, name: "eventToRelay");
				delay = s.Serialize<float>(delay, name: "delay");
				replaceSender = s.Serialize<bool>(replaceSender, name: "replaceSender", options: CSerializerObject.Options.BoolAsByte);
				replaceSenderByActivator = s.Serialize<bool>(replaceSenderByActivator, name: "replaceSenderByActivator", options: CSerializerObject.Options.BoolAsByte);
				resetTimerOnRetrigger = s.Serialize<bool>(resetTimerOnRetrigger, name: "resetTimerOnRetrigger", options: CSerializerObject.Options.BoolAsByte);
			}
		}
	}
}

