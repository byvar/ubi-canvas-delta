namespace UbiArt.ITF {
	public partial class PhysShapeBox {
		public void SetExtent(Vec2d vec, Vec2d scale = null) {
			Extent = vec;
			if(scale == null) scale = Vec2d.One;
			Points = new CListO<Vec2d>() {
				new Vec2d(-vec.x, -vec.y) * scale,
				new Vec2d(-vec.x, vec.y) * scale,
				new Vec2d(vec.x, vec.y) * scale,
				new Vec2d(vec.x, -vec.y) * scale
			};
			BuildEdges();
		}

		public override void Reset() {
			SetExtent(Vec2d.One);
		}
	}
}
