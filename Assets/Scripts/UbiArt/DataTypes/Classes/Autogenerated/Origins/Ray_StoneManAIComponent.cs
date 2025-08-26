namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_StoneManAIComponent : Ray_GroundEnemyAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE2C8051C;
	}
}

