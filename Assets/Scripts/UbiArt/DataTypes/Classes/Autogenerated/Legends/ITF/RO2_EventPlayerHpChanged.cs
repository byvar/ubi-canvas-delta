namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventPlayerHpChanged : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB824FD85;
	}
}

