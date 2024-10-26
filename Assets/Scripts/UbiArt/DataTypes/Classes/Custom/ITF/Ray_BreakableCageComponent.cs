namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BreakableCageComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA9F50599;
	}
}

