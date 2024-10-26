namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_PlayerCostumePriceComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2D587170;
	}
}

