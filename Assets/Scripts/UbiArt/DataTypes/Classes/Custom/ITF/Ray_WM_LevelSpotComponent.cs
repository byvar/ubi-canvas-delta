namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_LevelSpotComponent : Ray_WM_SpotComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF119E076;
	}
}

