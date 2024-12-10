namespace UbiArt.ITF {
	[Games(GameFlags.RO, platforms: PlatformFlags.Vita)] // Only on Vita!
	public partial class RayVita_FrescoManagerAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6ED6191E;
	}
}
