namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_EventHandsCaught : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBF5613FA;
	}
}

