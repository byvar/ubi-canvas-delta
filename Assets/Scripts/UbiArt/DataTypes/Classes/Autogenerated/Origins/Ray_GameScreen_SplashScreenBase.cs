namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_GameScreen_SplashScreenBase : GameScreenBase {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD81230A8;
	}
}

