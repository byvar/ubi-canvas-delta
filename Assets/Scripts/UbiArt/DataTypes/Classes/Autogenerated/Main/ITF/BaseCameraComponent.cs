namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RAVersion)]
	public partial class BaseCameraComponent : ActorComponent {
		public int remote;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL || s.Settings.Game == Game.VH) {
				remote = s.Serialize<int>(remote, name: "remote");
			} else {
			}
		}
		public override uint? ClassCRC => 0x71405453;
	}
}

