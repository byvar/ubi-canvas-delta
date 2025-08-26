namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventScoreGuageAddLum : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA8E76C7D;
	}
}

