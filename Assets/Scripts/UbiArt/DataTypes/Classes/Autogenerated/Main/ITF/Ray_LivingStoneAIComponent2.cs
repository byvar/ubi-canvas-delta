namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_LivingStoneAIComponent2 : Ray_GroundEnemyAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB226B58A;
	}
}

