namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_MusicScoreOpenEvent : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA3AA6A54;
	}
}

