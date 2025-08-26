namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_EventMedalCinematicEnded : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4755F77A;
	}
}

