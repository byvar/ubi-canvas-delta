namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_LivingstoneHatAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE2E87752;
	}
}

