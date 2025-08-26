namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventAttackGranted : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB6489BCD;
	}
}

