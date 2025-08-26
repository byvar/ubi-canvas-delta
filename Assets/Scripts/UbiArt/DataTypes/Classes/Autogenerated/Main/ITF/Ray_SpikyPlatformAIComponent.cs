namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_SpikyPlatformAIComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC9A8A7CB;
	}
}

