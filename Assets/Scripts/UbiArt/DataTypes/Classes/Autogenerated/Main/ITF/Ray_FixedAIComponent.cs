namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_FixedAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE3857746;
	}
}

