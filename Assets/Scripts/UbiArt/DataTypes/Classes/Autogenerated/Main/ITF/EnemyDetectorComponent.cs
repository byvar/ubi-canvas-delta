namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class EnemyDetectorComponent : ShapeDetectorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE7590898;
	}
}

