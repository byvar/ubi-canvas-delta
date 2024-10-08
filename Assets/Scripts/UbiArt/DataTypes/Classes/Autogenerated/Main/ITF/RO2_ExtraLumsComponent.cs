namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_ExtraLumsComponent : ActorComponent {
		public float disappearStartTime = -1f;
		public float disappearIntervalTime = -1f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				disappearStartTime = s.Serialize<float>(disappearStartTime, name: "disappearStartTime");
				disappearIntervalTime = s.Serialize<float>(disappearIntervalTime, name: "disappearIntervalTime");
			}
		}
		public override uint? ClassCRC => 0x8A1EAD12;
	}
}

