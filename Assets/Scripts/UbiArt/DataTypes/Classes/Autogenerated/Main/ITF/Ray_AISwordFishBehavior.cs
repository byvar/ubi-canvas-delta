namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AISwordFishBehavior : Ray_AIGroundBaseBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x515AB00D;
	}
}

