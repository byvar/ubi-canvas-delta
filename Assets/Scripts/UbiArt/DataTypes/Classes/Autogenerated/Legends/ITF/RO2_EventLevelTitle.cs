namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventLevelTitle : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5F8B0BFC;
	}
}

