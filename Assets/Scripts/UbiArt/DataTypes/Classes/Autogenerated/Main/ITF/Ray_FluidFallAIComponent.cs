namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_FluidFallAIComponent : GraphicComponent {
		public UV_MODE uvMode;
		public int startOpen;
		public float widthStart;
		public float widthBase;
		public float speed;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			uvMode = s.Serialize<UV_MODE>(uvMode, name: "uvMode");
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startOpen = s.Serialize<int>(startOpen, name: "startOpen");
				widthStart = s.Serialize<float>(widthStart, name: "widthStart");
				widthBase = s.Serialize<float>(widthBase, name: "widthBase");
				speed = s.Serialize<float>(speed, name: "speed");
			}
		}
		public enum UV_MODE {
			[Serialize("UV_MODE_OPTIMUM")] OPTIMUM = 0,
			[Serialize("UV_MODE_SPEED"  )] SPEED = 1,
		}
		public override uint? ClassCRC => 0x7FC096B9;
	}
}

