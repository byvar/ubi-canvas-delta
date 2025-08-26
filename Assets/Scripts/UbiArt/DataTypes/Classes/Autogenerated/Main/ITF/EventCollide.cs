namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventCollide : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBA1E05A3;
	}
}

