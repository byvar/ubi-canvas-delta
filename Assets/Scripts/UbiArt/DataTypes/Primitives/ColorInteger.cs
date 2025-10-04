using System;

namespace UbiArt {
	public class ColorInteger : ICSerializable, IEquatable<ColorInteger> {
		public uint colorBytes;

		public ColorInteger() {
		}
		public ColorInteger(ColorInteger c) {
			colorBytes = c?.colorBytes ?? 0;
		}
		public ColorInteger(ColorInteger c, byte a) {
			colorBytes = c?.colorBytes ?? 0;
			colorBytes = (colorBytes & 0xFFFFFF) | (((uint)a) << 24);
		}
		public ColorInteger(uint colorBytes) {
			this.colorBytes = colorBytes;
		}
		public ColorInteger(float r, float g, float b, float a) {
			uint rc = Math.Max(0, Math.Min(255, (uint)(r * 255f))) & 0xFF;
			uint gc = Math.Max(0, Math.Min(255, (uint)(g * 255f))) & 0xFF;
			uint bc = Math.Max(0, Math.Min(255, (uint)(b * 255f))) & 0xFF;
			uint ac = Math.Max(0, Math.Min(255, (uint)(a * 255f))) & 0xFF;
			colorBytes = (bc | (gc << 8) | (rc << 16) | (ac << 24));
		}

		public float A => ByteA / 255f;
		public float R => ByteR / 255f;
		public float G => ByteG / 255f;
		public float B => ByteB / 255f;
		// Order: A R G B (top bits -> bottom bits)
		public byte ByteA => (byte)((colorBytes >> 24) & 0xFF);
		public byte ByteR => (byte)((colorBytes >> 16) & 0xFF);
		public byte ByteG => (byte)((colorBytes >> 8) & 0xFF);
		public byte ByteB => (byte)((colorBytes >> 0) & 0xFF);

		public void Serialize(CSerializerObject s, string name) {
			colorBytes = s.Serialize<uint>(colorBytes);
		}

		public override string ToString() {
			return $"ColorInteger({R}, {G}, {B}, {A})";
		}

		#region Equals
		public override bool Equals(object obj) {
			return obj is ColorInteger && this == (ColorInteger)obj;
		}
		public override int GetHashCode() {
			return colorBytes.GetHashCode();
		}

		public bool Equals(ColorInteger other) {
			return this == (ColorInteger)other;
		}

		public static bool operator ==(ColorInteger a, ColorInteger b) {
			if (ReferenceEquals(a, b)) return true;
			if (ReferenceEquals(a, null)) return false;
			if (ReferenceEquals(b, null)) return false;
			return a.colorBytes == b.colorBytes;
		}
		public static bool operator !=(ColorInteger x, ColorInteger y) {
			return !(x == y);
		}
		#endregion
	}
}
