namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_WorldSpotComponent : Ray_WM_SpotComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBB4CE09E;
	}
}

