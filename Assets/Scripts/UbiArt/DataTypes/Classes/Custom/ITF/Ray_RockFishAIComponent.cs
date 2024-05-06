namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_RockFishAIComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5F7ED5B2;
	}
}

