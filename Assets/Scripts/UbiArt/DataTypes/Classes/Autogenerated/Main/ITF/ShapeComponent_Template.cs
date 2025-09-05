namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class ShapeComponent_Template : ActorComponent_Template {
		public CListO<StringID> AnimPolylineList;
		public Generic<PhysShape> shape;
		public Vec2d offset;
		public StringID attachPolyline;
		public StringID proceduralBone;
		public CListO<ShapeData_Template> shapes;
		public bool useAABBShape;
		public StringID bone;
		public StringID bone3D;
		public StringID polyline;
		public int drawDebug;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR) {
				polyline = s.SerializeObject<StringID>(polyline, name: "polyline");
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				attachPolyline = s.SerializeObject<StringID>(attachPolyline, name: "attachPolyline");
				shapes = s.SerializeObject<CListO<ShapeData_Template>>(shapes, name: "shapes");
				useAABBShape = s.Serialize<bool>(useAABBShape, name: "useAABBShape");
			} else if (s.Settings.Game == Game.RO) {
				polyline = s.SerializeObject<StringID>(polyline, name: "polyline");
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				attachPolyline = s.SerializeObject<StringID>(attachPolyline, name: "attachPolyline");
				if (!s.HasProperties(SerializerProperties.Binary) && s.HasFlags(SerializeFlags.Group_Data)) {
					drawDebug = s.Serialize<int>(drawDebug, name: "drawDebug");
				}
				shapes = s.SerializeObject<CListO<ShapeData_Template>>(shapes, name: "shapes");
				useAABBShape = s.Serialize<bool>(useAABBShape, name: "useAABBShape");
			} else if (s.Settings.Game == Game.RL) {
				polyline = s.SerializeObject<StringID>(polyline, name: "polyline");
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				attachPolyline = s.SerializeObject<StringID>(attachPolyline, name: "attachPolyline");
				proceduralBone = s.SerializeObject<StringID>(proceduralBone, name: "proceduralBone");
				shapes = s.SerializeObject<CListO<ShapeData_Template>>(shapes, name: "shapes");
				useAABBShape = s.Serialize<bool>(useAABBShape, name: "useAABBShape");
				bone = s.SerializeObject<StringID>(bone, name: "bone");
				if (s.Settings.Platform != GamePlatform.Vita) {
					bone3D = s.SerializeObject<StringID>(bone3D, name: "bone3D");
				}
			} else if (s.Settings.Game == Game.COL) {
				polyline = s.SerializeObject<StringID>(polyline, name: "polyline");
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				attachPolyline = s.SerializeObject<StringID>(attachPolyline, name: "attachPolyline");
				proceduralBone = s.SerializeObject<StringID>(proceduralBone, name: "proceduralBone");
				shapes = s.SerializeObject<CListO<ShapeData_Template>>(shapes, name: "shapes");
				useAABBShape = s.Serialize<bool>(useAABBShape, name: "useAABBShape", options: CSerializerObject.Options.BoolAsByte);
				bone = s.SerializeObject<StringID>(bone, name: "bone");
				bone3D = s.SerializeObject<StringID>(bone3D, name: "bone3D");
			} else {
				AnimPolylineList = s.SerializeObject<CListO<StringID>>(AnimPolylineList, name: "AnimPolylineList");
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				offset = s.SerializeObject<Vec2d>(offset, name: "offset");
				attachPolyline = s.SerializeObject<StringID>(attachPolyline, name: "attachPolyline");
				proceduralBone = s.SerializeObject<StringID>(proceduralBone, name: "proceduralBone");
				shapes = s.SerializeObject<CListO<ShapeData_Template>>(shapes, name: "shapes");
				useAABBShape = s.Serialize<bool>(useAABBShape, name: "useAABBShape");
				bone = s.SerializeObject<StringID>(bone, name: "bone");
				bone3D = s.SerializeObject<StringID>(bone3D, name: "bone3D");
			}
		}
		public override uint? ClassCRC => 0x06B15761;
	}
}

