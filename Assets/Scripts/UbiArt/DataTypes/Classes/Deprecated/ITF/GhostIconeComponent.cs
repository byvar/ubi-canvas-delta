namespace UbiArt.ITF {
	[Games(GameFlags.RO, platforms: PlatformFlags.Vita)] // Only on Vita!
	public partial class GhostIconeComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x05EEFC41;
	}
}
