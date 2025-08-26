namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_EventQueryLumCount : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE23199EA;
	}
}

