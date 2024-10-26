namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_HomeTreeBrickComponent_Template : ActorComponent_Template {
		public uint editor_stepCount = 20;
		public float editor_minGrowth = 0.2f;
		public Vec2d editor_brickSize = new Vec2d(40, 20);
		public float editor_trunkWidth = 20;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			editor_stepCount = s.Serialize<uint>(editor_stepCount, name: "editor_stepCount");
			editor_minGrowth = s.Serialize<float>(editor_minGrowth, name: "editor_minGrowth");
			editor_brickSize = s.SerializeObject<Vec2d>(editor_brickSize, name: "editor_brickSize");
			editor_trunkWidth = s.Serialize<float>(editor_trunkWidth, name: "editor_trunkWidth");
		}
		public override uint? ClassCRC => 0xD1777642;
	}
}

