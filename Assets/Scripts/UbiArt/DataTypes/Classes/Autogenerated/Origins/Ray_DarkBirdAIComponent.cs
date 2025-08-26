namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DarkBirdAIComponent : Ray_GroundEnemyAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1EFE60A5;
	}
}

