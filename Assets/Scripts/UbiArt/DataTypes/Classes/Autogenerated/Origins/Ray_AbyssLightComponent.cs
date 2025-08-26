namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AbyssLightComponent : LightComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBB1C66ED;
	}
}

