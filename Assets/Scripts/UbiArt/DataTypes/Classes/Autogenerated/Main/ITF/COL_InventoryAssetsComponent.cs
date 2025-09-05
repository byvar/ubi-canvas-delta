namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InventoryAssetsComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA827A6FA;
	}
}

