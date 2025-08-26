namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_RewardAIComponent : AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF0178271;
	}
}

