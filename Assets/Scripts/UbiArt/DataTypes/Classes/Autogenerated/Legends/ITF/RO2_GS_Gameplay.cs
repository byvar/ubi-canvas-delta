namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RM)]
	public partial class RO2_GS_Gameplay : RO2_GameScreen {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB360CC34;
	}
}

