namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class SolidStim : EventStim {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBB7014DB;
	}
}

