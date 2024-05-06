using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class AABB {
		public void SetPoint(Vec2d point) {
			MIN = new Vec2d(point.x, point.y);
			MAX = new Vec2d(point.x, point.y);
		}
		public void Grow(Vec2d point) {
			if(point == null) return;
			if (point.x < MIN.x) MIN.x = point.x;
			if (point.y < MIN.y) MIN.y = point.y;
			if (point.x > MAX.x) MAX.x = point.x;
			if (point.y > MAX.y) MAX.y = point.y;

		}
		public void Grow(Vec3d point) {
			if (point == null) return;
			Grow(new Vec2d(point.x, point.y));
		}
		public void Grow(AABB aabb) {
			Grow(aabb?.MIN);
			Grow(aabb?.MAX);
		}
		public AABB Transform(Vec2d position, Angle rotation, Vec2d scale) {
			Vec2d[] points = new Vec2d[] {
				(new Vec2d(MIN.x, MIN.y).Rotate(rotation) * scale) + position,
				(new Vec2d(MIN.x, MAX.y).Rotate(rotation) * scale) + position,
				(new Vec2d(MAX.x, MAX.y).Rotate(rotation) * scale) + position,
				(new Vec2d(MAX.x, MIN.y).Rotate(rotation) * scale) + position,
			};
			return new AABB() {
				MIN = new Vec2d(points.Min(p => p.x), points.Min(p => p.y)),
				MAX = new Vec2d(points.Max(p => p.x), points.Max(p => p.y))
			};
		}
		public static AABB operator +(AABB a, Vec2d b) => a.Transform(b, 0, Vec2d.One);
		public static AABB operator -(AABB a, Vec2d b) => a.Transform(-b, 0, Vec2d.One);
		public static AABB operator *(AABB a, Vec2d b) => a.Transform(Vec2d.Zero, 0, b);
		public static AABB operator /(AABB a, Vec2d b) => a.Transform(Vec2d.Zero, 0, Vec2d.One / b);
		public static AABB operator *(AABB a, float b) => a.Transform(Vec2d.Zero, 0, Vec2d.One * b);
		public static AABB operator /(AABB a, float b) => a.Transform(Vec2d.Zero, 0, Vec2d.One / b);
		public static AABB operator -(AABB a) => a.Transform(Vec2d.Zero, 0, -Vec2d.One);
	}
}
