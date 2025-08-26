namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIShooterReceiveHitAction : Ray_AIReceiveHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x896E0727;
	}
}

