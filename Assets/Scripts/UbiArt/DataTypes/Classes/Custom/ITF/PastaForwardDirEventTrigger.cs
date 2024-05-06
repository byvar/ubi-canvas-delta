namespace UbiArt.ITF {
	[Games(GameFlags.RJR)]
	public partial class PastaForwardDirEventTrigger : EventTrigger {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5B4D1307;
	}
}

