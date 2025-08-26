namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIFlyIdleAction : AIAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE191B9F0;
	}
}

