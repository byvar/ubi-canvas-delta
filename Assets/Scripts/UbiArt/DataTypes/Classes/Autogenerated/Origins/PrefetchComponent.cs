namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PrefetchComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE32B0606;
	}
}

