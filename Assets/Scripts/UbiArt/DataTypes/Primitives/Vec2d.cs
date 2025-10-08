using System;
using System.Runtime.Serialization;

namespace UbiArt {
	public class Vec2d : ICSerializable, IEquatable<Vec2d> {
		public float x;
		public float y;

		public Vec2d() {
		}
		public Vec2d(Vec2d v) {
			x = v.x;
			y = v.y;
		}
		public Vec2d(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void Serialize(CSerializerObject s, string name) {
			x = s.Serialize<float>(x);
			y = s.Serialize<float>(y);
		}

		public override string ToString() {
			return $"Vec2d({x}, {y})";
		}

		public static Vec2d Zero => new Vec2d();
		public static Vec2d One => new Vec2d(1f, 1f);
		public static Vec2d MinusInfinity => new Vec2d(float.MinValue, float.MinValue);
		public static Vec2d Infinity => new Vec2d(float.MaxValue, float.MaxValue); // 0x7F7FFFFF: NOT float.PositiveInfinity!

		[IgnoreDataMember]
		public double NormDouble => Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		[IgnoreDataMember]
		public float Norm => MathF.Sqrt(SquareNorm);
		[IgnoreDataMember]
		public float SquareNorm => y * y + x * x;
		[IgnoreDataMember]
		public Vec2d Rotate90 => new Vec2d(-y, x);
		[IgnoreDataMember]
		public float Angle => MathF.Atan2(y, x);

		public static float Dot(Vec2d a, Vec2d b) => a.x * b.x + a.y * b.y;
		public static float Cross(Vec2d a, Vec2d b) => a.x * b.y - b.x * a.y;
		public static Vec2d operator +(Vec2d a, Vec2d b) => new Vec2d(a.x + b.x, a.y + b.y);
		public static Vec2d operator -(Vec2d a, Vec2d b) => new Vec2d(a.x - b.x, a.y - b.y);
		public static Vec2d operator *(Vec2d a, Vec2d b) => new Vec2d(a.x * b.x, a.y * b.y);
		public static Vec2d operator /(Vec2d a, Vec2d b) => new Vec2d(a.x / b.x, a.y / b.y);
		public static Vec2d operator *(Vec2d a, float b) => new Vec2d(a.x * b, a.y * b);
		public static Vec2d operator /(Vec2d a, float b) => new Vec2d(a.x / b, a.y / b);
		public static Vec2d operator -(Vec2d a) => new Vec2d(-a.x, -a.y);
		public Vec2d NormalizeDouble() => (x != 0 || y != 0) ? this / (float)NormDouble : this;
		public Vec2d Normalize() {
			var magnitudeFloat = Norm;
			if(magnitudeFloat <= MIN_NORM)
				return Vec2d.Zero;
			else
				return this / magnitudeFloat;
		}
		public bool IsEqual(Vec2d b, float margin) {
			return MathF.Abs(x - (b?.x ?? 0)) <= margin &&
				   MathF.Abs(y - (b?.y ?? 0)) <= margin;
		}

		const float MIN_NORM = 0.00001f;

		// In radians
		public Vec2d Rotate(float angle) {
			var cos = MathF.Cos(angle);
			var sin = MathF.Sin(angle);
			return new Vec2d(
				x * cos - y * sin,
				x * sin + y * cos);
		}
		public Vec2d Rotate(Vec2d cosSin) {
			return new Vec2d(
				x * cosSin.x - y * cosSin.y,
				x * cosSin.y + y * cosSin.x);
		}
		public Vec2d RotateAround(Vec2d center, float angle)
			=> center + ((this - center).Rotate(angle));

		public Vec3d ToVec3d(float z) => new Vec3d(x, y, z);

		public static Vec2d Lerp(Vec2d a, Vec2d b, float lerp) => a + ((b - a) * lerp);

		public static Vec2d Left => new Vec2d(-1,0);
		public static Vec2d Right => new Vec2d(1, 0);
		public static Vec2d Up => new Vec2d(0, 1);
		public static Vec2d Down => new Vec2d(0, -1);
		public static Vec2d XAxis => new Vec2d(1, 0);
		public static Vec2d YAxis => new Vec2d(0, 1);

		#region Equals
		public override bool Equals(object obj) {
			return obj is Vec2d && this == (Vec2d)obj;
		}
		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode() << 2;
		}

		public bool Equals(Vec2d other) {
			return this == (Vec2d)other;
		}

		public static bool operator ==(Vec2d a, Vec2d b) {
			if (ReferenceEquals(a, b)) return true;
			if (ReferenceEquals(a, null)) return false;
			if (ReferenceEquals(b, null)) return false;
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Vec2d x, Vec2d y) {
			return !(x == y);
		}
		#endregion
	}
}