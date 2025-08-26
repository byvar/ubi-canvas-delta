namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventScoreSetup : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x197F1CDE;
	}
}

