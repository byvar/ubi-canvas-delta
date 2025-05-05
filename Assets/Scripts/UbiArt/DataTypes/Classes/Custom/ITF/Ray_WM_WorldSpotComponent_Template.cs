namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_WorldSpotComponent_Template : Ray_WM_SpotComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x97131C70;
	}
}

