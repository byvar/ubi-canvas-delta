namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_GameStatsComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5BDBE583;
	}
}

