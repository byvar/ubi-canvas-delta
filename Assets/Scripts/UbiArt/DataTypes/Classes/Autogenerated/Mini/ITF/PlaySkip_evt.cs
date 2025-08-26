namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RM)]
	public partial class PlaySkip_evt : SequenceEvent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9FC4A336;
	}
}

