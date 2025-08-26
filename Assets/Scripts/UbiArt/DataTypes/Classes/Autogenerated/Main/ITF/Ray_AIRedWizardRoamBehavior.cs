namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIRedWizardRoamBehavior : Ray_AIGroundRoamBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x7327D633;
	}
}

