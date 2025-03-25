namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_AdversarialSkullCoinModeComponent_Template : RO2_AdversarialModeComponent_Template {
		public Path skullCoinPath;
		public int canSteal;
		public float stealingImmunityDuration = 1;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			skullCoinPath = s.Serialize<Path>(skullCoinPath, name: "skullCoinPath");
			canSteal = s.Serialize<int>(canSteal, name: "canSteal");
			stealingImmunityDuration = s.Serialize<float>(stealingImmunityDuration, name: "stealingImmunityDuration");
		}
		public override uint? ClassCRC => 0x97223F2C;
	}
}

