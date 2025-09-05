namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MonitorStoryEventInteraction : COL_ObjectInteraction {
		public StringID storyEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			storyEvent = s.SerializeObject<StringID>(storyEvent, name: "storyEvent");
		}
		public override uint? ClassCRC => 0xDFC741F1;
	}
}

