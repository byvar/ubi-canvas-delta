namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class EventQueryWaterInfluence : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x522B8E1C;
	}
}

