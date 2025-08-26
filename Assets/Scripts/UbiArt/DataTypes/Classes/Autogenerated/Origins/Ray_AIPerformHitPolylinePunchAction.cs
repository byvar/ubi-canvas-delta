namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIPerformHitPolylinePunchAction : Ray_AIPerformHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC576E5D4;
	}
}

