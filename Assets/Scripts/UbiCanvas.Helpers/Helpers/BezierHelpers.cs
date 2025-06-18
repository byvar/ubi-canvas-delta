using System;
using UbiArt;

namespace UbiCanvas.Helpers {
	public static class BezierHelpers {
		public static Vec2d CalculateCubicBezierPoint(float t, Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3) {
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;
			float uuu = uu * u;
			float ttt = tt * t;

			Vec2d p = p0 * uuu;
			p += p1 * 3f * uu * t;
			p += p2 * 3f * u * tt;
			p += p3 * ttt;

			return p;
		}
		public static Vec2d CalculateCubicBezierPoint(float t, Vec2d[] points) {
			return CalculateCubicBezierPoint(t, points[0], points[1], points[2], points[3]);
		}
		public static Vec2d[] GetControlPoints(Vec2d point0, Vec2d point1, Vec2d normal0, Vec2d normal1, bool flip0 = false, bool flip1 = false) {
			var N01 = (float)(point0 - point1).Magnitude * 0.5f;
			return new Vec2d[] {
				point0,
				point0 + new Vec2d(-normal0.y, normal0.x) * N01 * (flip0 ? -1 : 1),
				point1 - new Vec2d(-normal1.y, normal1.x) * N01 * (flip1 ? -1 : 1),
				point1
			};
		}
		public static Vec2d[] GetControlPoints<T>(T point0, T point1, Func<T, Vec2d> pointFunc, Func<T, Vec2d> normalFunc, bool flip0 = false, bool flip1 = false) {
			return GetControlPoints(
				pointFunc(point0),
				pointFunc(point1),
				normalFunc(point0),
				normalFunc(point1),
				flip0: flip0,
				flip1: flip1);
		}
	}
}
