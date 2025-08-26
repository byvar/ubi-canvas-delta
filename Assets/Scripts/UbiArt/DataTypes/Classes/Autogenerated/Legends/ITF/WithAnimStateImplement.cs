namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion)]
	public partial class WithAnimStateImplement : BasicStateImplement {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3330381D;
	}
}

