using System;

namespace UbiArt {
	public class Angle : ICSerializable, IEquatable<Angle> {
		public float angle;

		public void Serialize(CSerializerObject s, string name) {
			angle = s.Serialize<float>(angle);
		}

		// Constructors
		public Angle() { }
		public Angle(float angle, bool degrees = false) {
			if (degrees) {
				EulerAngle = angle;
			} else {
				this.angle = angle;
			}
		}

		// Casts
		public static implicit operator float(Angle a) {
			return a?.angle ?? 0;
		}
		public static implicit operator Angle(float a) {
			return new Angle { angle = a };
		}
		public static implicit operator Angle(AngleAmount a) {
			return new Angle { angle = a?.angle ?? 0f };
		}

		public float EulerAngle {
			get {
				return angle * Rad2Deg;
			}
			set {
				angle = value * Deg2Rad;
			}
		}
		public override string ToString() => $"Angle({angle}rad|{EulerAngle}°)";

		private readonly float Deg2Rad = (MathF.PI * 2) / 360f;
		private readonly float Rad2Deg = 360f / (MathF.PI * 2);


		#region Equals
		public override bool Equals(object obj) {
			return obj is Angle && this == (Angle)obj;
		}
		public override int GetHashCode() {
			return angle.GetHashCode();
		}

		public bool Equals(Angle other) {
			return this == (Angle)other;
		}

		public static bool operator ==(Angle a, Angle b) {
			if (ReferenceEquals(a, b)) return true;
			if (ReferenceEquals(a, null)) return false;
			if (ReferenceEquals(b, null)) return false;
			return a.angle == b.angle;
		}
		public static bool operator !=(Angle x, Angle y) {
			return !(x == y);
		}
		#endregion
	}
}
