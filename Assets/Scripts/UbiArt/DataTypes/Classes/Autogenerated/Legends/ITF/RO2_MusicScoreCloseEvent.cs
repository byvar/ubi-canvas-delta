namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_MusicScoreCloseEvent : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x27A7306D;
	}
}

