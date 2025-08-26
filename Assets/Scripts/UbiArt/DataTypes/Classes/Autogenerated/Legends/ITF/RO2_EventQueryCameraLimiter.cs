namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_EventQueryCameraLimiter : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3331A0BA;
	}
}

