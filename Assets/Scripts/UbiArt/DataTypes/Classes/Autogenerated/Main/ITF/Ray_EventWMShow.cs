namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventWMShow : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7D4E599D;
	}
}

