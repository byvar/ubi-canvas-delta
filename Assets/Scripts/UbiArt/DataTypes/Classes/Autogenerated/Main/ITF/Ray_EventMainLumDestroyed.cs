namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventMainLumDestroyed : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xABD7B1BC;
	}
}

