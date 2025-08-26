namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class WindStim : EventStim {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x78C995CF;
	}
}

