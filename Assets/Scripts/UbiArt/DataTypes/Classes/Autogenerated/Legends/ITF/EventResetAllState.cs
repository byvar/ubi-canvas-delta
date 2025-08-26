namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class EventResetAllState : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBF9DFFB5;
	}
}

