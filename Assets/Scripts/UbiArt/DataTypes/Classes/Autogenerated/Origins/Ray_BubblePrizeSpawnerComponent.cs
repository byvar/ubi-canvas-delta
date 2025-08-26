namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BubblePrizeSpawnerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x656E616D;
	}
}

