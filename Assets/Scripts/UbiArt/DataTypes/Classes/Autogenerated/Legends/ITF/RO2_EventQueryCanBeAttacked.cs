namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventQueryCanBeAttacked : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD4472ADF;
	}
}

