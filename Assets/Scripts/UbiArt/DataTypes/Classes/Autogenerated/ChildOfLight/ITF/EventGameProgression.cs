namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class EventGameProgression : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3DED62B5;
	}
}

