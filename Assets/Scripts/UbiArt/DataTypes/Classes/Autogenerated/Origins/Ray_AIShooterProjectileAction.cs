namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIShooterProjectileAction : AIAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8620A51C;
	}
}

