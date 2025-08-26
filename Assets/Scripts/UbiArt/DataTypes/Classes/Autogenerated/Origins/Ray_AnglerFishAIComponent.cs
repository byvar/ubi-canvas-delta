namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AnglerFishAIComponent : Ray_SimpleAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x18F7CC8D;
	}
}

