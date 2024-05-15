namespace UbiArt.UV {
	public class UVparameters : CSerializable {
		public float orientation;
		public string flag;
		public CArrayO<Parameters> parameters; // Number of vertices used by the triangles
		public CArrayO<Triangle> triangles;

		protected override void SerializeImpl(CSerializerObject s) {
			orientation = s.Serialize<float>(orientation, name: "orientation");
			flag = s.Serialize<string>(flag, name: "flag");
			parameters = s.SerializeObject<CArrayO<Parameters>>(parameters, name: "parameters");
			triangles = s.SerializeObject<CArrayO<Triangle>>(triangles, name: "triangles");
		}
		public class Triangle : CSerializable {
			public int vertex1;
			public int vertex2;
			public int vertex3;
			protected override void SerializeImpl(CSerializerObject s) {
				vertex1 = s.Serialize<int>(vertex1, name: "vertex1");
				vertex2 = s.Serialize<int>(vertex2, name: "vertex2");
				vertex3 = s.Serialize<int>(vertex3, name: "vertex3");
			}
		}
		public class Parameters : CSerializable {
			public float weight;
			public float depth;
			protected override void SerializeImpl(CSerializerObject s) {
				weight = s.Serialize<float>(weight, name: "weight");
				depth = s.Serialize<float>(depth, name: "depth");
			}
		}
	}
}
