namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIBounceRoamingBehavior : AIBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x33B5F3D3;
	}
}

