namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RAVersion)]
	public partial class EventSwimingPlouf : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE76B0BD4;
	}
}

