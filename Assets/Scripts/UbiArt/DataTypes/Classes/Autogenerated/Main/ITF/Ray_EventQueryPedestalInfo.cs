namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_EventQueryPedestalInfo : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7D9CA5AC;
	}
}

