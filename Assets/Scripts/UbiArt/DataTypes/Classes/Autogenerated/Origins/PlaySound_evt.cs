namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PlaySound_evt : SequenceEventWithActor {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9D874AFD;
	}
}

