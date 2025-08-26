namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIPerformHitPunchAction : Ray_AIPerformHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1FC77FA1;
	}
}

