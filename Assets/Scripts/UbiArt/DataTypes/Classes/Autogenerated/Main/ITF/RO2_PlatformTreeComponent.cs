namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_PlatformTreeComponent : ActorComponent {
		public bool startActivated = true;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startActivated = s.Serialize<bool>(startActivated, name: "startActivated");
			}
		}
		public override uint? ClassCRC => 0x7EC341B5;
	}
}

