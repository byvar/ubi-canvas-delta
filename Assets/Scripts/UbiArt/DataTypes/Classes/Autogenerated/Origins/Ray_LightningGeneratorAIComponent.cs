namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_LightningGeneratorAIComponent : TimedSpawnerAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA1031858;
	}
}

