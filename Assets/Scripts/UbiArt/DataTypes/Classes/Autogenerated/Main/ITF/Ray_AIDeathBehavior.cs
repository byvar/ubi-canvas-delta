namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIDeathBehavior : AIDeathBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x79DB7929;
	}
}

