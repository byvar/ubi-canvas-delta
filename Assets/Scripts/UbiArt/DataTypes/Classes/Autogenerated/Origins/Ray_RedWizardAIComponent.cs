namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_RedWizardAIComponent : Ray_GroundAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x139F587B;
	}
}

