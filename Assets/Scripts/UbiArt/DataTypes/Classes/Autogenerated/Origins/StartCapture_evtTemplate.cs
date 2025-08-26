namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class StartCapture_evtTemplate : SequenceEvent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7B90C400;
	}
}

