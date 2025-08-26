namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RM)]
	public partial class AIFallAction : AIAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x24E0E50F;
	}
}

