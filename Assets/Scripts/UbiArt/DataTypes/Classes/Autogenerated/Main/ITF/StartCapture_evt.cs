namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class StartCapture_evt : SequenceEvent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA23C57DD;
	}
}

