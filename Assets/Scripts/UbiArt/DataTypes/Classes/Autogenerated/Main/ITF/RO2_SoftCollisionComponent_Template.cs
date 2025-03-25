namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_SoftCollisionComponent_Template : GraphicComponent_Template {
		public float CellSpace = 1;
		public float Gravity = 9.81f;
		public float MassCoeff = 1;
		public float Radius = 0.5f;
		public bool DrawParticles;
		public bool DrawGrid;
		public bool DrawOwnerCells;
		public bool DrawCollision;
		public GFXMaterialSerializable Material;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			CellSpace = s.Serialize<float>(CellSpace, name: "CellSpace");
			Gravity = s.Serialize<float>(Gravity, name: "Gravity");
			MassCoeff = s.Serialize<float>(MassCoeff, name: "MassCoeff");
			Radius = s.Serialize<float>(Radius, name: "Radius");
			DrawParticles = s.Serialize<bool>(DrawParticles, name: "DrawParticles");
			DrawGrid = s.Serialize<bool>(DrawGrid, name: "DrawGrid");
			DrawOwnerCells = s.Serialize<bool>(DrawOwnerCells, name: "DrawOwnerCells");
			DrawCollision = s.Serialize<bool>(DrawCollision, name: "DrawCollision");
			Material = s.SerializeObject<GFXMaterialSerializable>(Material, name: "Material");
		}
		public override uint? ClassCRC => 0x451F0B30;
	}
}

