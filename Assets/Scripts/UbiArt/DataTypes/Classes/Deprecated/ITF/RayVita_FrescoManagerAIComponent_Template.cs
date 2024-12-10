namespace UbiArt.ITF {
	[Games(GameFlags.RO, platforms: PlatformFlags.Vita)] // Only on Vita!
	public partial class RayVita_FrescoManagerAIComponent_Template : Ray_AIComponent_Template {
		public int unknown;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			unknown = s.Serialize<int>(unknown, name: "unknown");
		}
		public override uint? ClassCRC => 0x63AEE81C;
	}
}
