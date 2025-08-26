namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_GroundAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x84F1A343;
	}
}

