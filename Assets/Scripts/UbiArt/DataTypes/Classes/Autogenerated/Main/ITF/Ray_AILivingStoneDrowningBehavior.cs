namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AILivingStoneDrowningBehavior : Ray_AIWaterBaseBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x660B48BF;
	}
}

