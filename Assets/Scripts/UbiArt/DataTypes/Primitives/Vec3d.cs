﻿using System;
using System.Runtime.Serialization;

namespace UbiArt {
	public class Vec3d : ICSerializable {
		public float x;
		public float y;
		public float z;

		public Vec3d() {}
		public Vec3d(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void Serialize(CSerializerObject s, string name) {
			x = s.Serialize<float>(x);
			y = s.Serialize<float>(y);
			z = s.Serialize<float>(z);
		}

		public override string ToString() => $"Vec3d({x}, {y}, {z})";

		public static Vec3d Zero => new Vec3d();
		public static Vec3d One => new Vec3d(1, 1, 1);
		public static Vec3d Invalid => new Vec3d(float.MaxValue, float.MaxValue, float.MaxValue);

		[IgnoreDataMember]
		public double NormDouble => Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
		[IgnoreDataMember]
		public float Norm => MathF.Sqrt(x*x + y*y + z*z);

		public static Vec3d operator +(Vec3d a, Vec3d b) => new Vec3d(a.x + b.x, a.y + b.y, a.z + b.z);
		public static Vec3d operator -(Vec3d a, Vec3d b) => new Vec3d(a.x - b.x, a.y - b.y, a.z - b.z);
		public static Vec3d operator *(Vec3d a, Vec3d b) => new Vec3d(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vec3d operator /(Vec3d a, Vec3d b) => new Vec3d(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vec3d operator *(Vec3d a, float b) => new Vec3d(a.x * b, a.y * b, a.z * b);
		public static Vec3d operator /(Vec3d a, float b) => new Vec3d(a.x / b, a.y / b, a.z / b);
		public static Vec3d operator -(Vec3d a) => new Vec3d(-a.x, -a.y, -a.z);
		public Vec3d NormalizeDouble() => (x != 0 || y != 0 || z != 0) ? this / (float)NormDouble : this;
		public Vec3d Normalize() {
			var magnitudeFloat = Norm;
			if (magnitudeFloat == 0)
				return Vec3d.Zero;
			else
				return this / magnitudeFloat;
		}

		public static Vec3d Left => new Vec3d(-1, 0, 0);
		public static Vec3d Right => new Vec3d(1, 0, 0);
		public static Vec3d Up => new Vec3d(0, 1, 0);
		public static Vec3d Down => new Vec3d(0, -1, 0);
		public static Vec3d Forward => new Vec3d(0, 0, 1);
		public static Vec3d Backward => new Vec3d(0, 0, -1);
	}
}
