namespace UbiArt.ITF {
	[Games(GameFlags.RJR)]
	public partial class PastaBackwardDirEventTrigger : EventTrigger {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6DCD096F;
	}
}

