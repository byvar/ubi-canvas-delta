namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PlayerStateMachine : WithAnimStateMachine {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6098EBDF;
	}
}

