namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class SoftPlatformComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC60F0FAF;
	}
}

