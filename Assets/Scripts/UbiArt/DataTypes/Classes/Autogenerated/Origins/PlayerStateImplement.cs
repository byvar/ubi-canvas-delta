namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PlayerStateImplement : WithAnimStateImplement {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5593D8E6;
	}
}

