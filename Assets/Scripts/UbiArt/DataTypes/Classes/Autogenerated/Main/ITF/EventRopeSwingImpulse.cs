namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class EventRopeSwingImpulse : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3027024E;
	}
}

