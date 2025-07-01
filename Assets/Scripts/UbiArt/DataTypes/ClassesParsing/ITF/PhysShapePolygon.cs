﻿namespace UbiArt.ITF {
	public partial class PhysShapePolygon {
		public void BuildEdges() {
			if (edge == null) edge = new CListO<Vec2d>();
			if (normals == null) normals = new CListO<Vec2d>();
			if (Points == null) Points = new CListO<Vec2d>();
			var count = Points.Count;
			if(distances == null || distances.Count != count) distances = new CArrayP<float>(new float[count]);
			edge.Clear();
			normals.Clear();
			for (int i = 0; i < count; i++) {
				var pt1 = Points[i];
				var pt2 = Points[(i+1) % count];
				distances[i] = (pt2 - pt1).Norm;
				if (distances[i] < 9.999999e-11) {
					distances[i] = 0;
					edge.Add(Vec2d.Zero);
					normals.Add(Vec2d.Zero);
				} else {
					var e = (pt2 - pt1).Normalize();
					edge.Add(e);
					normals.Add(new Vec2d(0 - e.y, e.x));
				}
			}
		}
		public virtual void Reset() {
			var extent = new Vec2d(1f, 1f);
			Points = new CListO<Vec2d>() {
					new Vec2d(-extent.x, -extent.y),
					new Vec2d(-extent.x, extent.y),
					new Vec2d(extent.x, extent.y),
					new Vec2d(extent.x, -extent.y)
				};
			BuildEdges();
		}
	}
}
