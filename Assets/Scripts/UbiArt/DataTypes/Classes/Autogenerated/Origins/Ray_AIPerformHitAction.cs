namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIPerformHitAction : AIPerformHitAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4B09B6B4;
	}
}

