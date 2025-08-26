namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class GameScreen_Initial : GameScreenBase {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA8C2D2D6;
	}
}

