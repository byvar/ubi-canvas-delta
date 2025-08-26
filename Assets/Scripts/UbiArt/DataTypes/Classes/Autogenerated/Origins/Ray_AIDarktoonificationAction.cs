namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIDarktoonificationAction : Ray_AIPerformHitPunchAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x01DD5A3A;
	}
}

