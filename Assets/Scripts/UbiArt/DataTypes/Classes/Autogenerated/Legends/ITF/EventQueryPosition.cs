namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventQueryPosition : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB87B76E3;
	}
}

