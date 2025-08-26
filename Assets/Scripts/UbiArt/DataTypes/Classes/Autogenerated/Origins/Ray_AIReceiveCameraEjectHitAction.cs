namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIReceiveCameraEjectHitAction : Ray_AIReceiveHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x000B15F8;
	}
}

