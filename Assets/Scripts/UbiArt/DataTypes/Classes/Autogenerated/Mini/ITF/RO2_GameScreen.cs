namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RM | GameFlags.RA)]
	public partial class RO2_GameScreen : GameScreen {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4C6F0092;
	}
}

