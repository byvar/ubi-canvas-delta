namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_EventBounceToLayer : Event {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD25B9DC7;
	}
}

