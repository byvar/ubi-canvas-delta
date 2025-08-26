namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class EventSingletonConfigChanged : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x01F5608C;
	}
}

