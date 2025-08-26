namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_ScoreLumAIComponent : Ray_FixedAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x935B2C9E;
	}
}

