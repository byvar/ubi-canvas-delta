namespace UbiArt.ITF {
	public partial class EventResetFollower : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}

		public override uint? ClassCRC => 0xE556FA06;
	}
}

