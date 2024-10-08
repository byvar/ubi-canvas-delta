namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_GeyserPlatformAIComponent : GraphicComponent {
		public int startOpen = -1;
		public float platformHeight = float.MaxValue;
		public float angle = float.MaxValue;
		public float width = 1f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startOpen = s.Serialize<int>(startOpen, name: "startOpen");
				platformHeight = s.Serialize<float>(platformHeight, name: "platformHeight");
				angle = s.Serialize<float>(angle, name: "angle");
				width = s.Serialize<float>(width, name: "width");
			}
		}
		public override uint? ClassCRC => 0x4890020D;
	}
}

