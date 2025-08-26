namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RAVersion)]
	public partial class InterpolatorComponent : ActorComponent {
		public StringID input;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game is Game.RA or Game.RM) {
				input = s.SerializeObject<StringID>(input, name: "input");
			}
		}
		public override uint? ClassCRC => 0x5AF6429A;
	}
}

