﻿using System.Globalization;

namespace UbiArt {
	public class Color : ICSerializable {
		public float b;
		public float g;
		public float r;
		public float a;

		public Color() {
		}
		public Color(float r, float g, float b, float a) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public void Serialize(CSerializerObject s, string name) {
			b = s.Serialize<float>(b);
			g = s.Serialize<float>(g);
			r = s.Serialize<float>(r);
			a = s.Serialize<float>(a);
		}

		public Color Alpha(float alpha) => new Color(r, g, b, alpha);

		public static Color Black => new Color(0, 0, 0, 1f);
		public static Color White => new Color(1, 1, 1, 1f);
		public static Color Red   => new Color(1, 0, 0, 1f);
		public static Color Green => new Color(0, 1, 0, 1f);
		public static Color Blue  => new Color(0, 0, 1, 1f);
		public static Color Grey => new Color(0.5f, 0.5f, 0.5f, 1f);
		public static Color Zero => new Color(0,0,0,0);
		public static Color operator *(Color a, float b) => new Color(a.r * b, a.g * b, a.b * b, a.a * b);
		public static Color operator *(Color a, Color b) => new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
		public static Color FromHTML(string colorcode) {
			float r = 0, g = 0, b = 0, a = 0;
			if (!string.IsNullOrEmpty(colorcode)) {
				colorcode = colorcode.TrimStart('#');

				if (colorcode.Length == 6) {
					r = int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber) / 255f;
					g = int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber) / 255f;
					b = int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber) / 255f;
					a = 1f;
				} else { // assuming length of 8
					r = int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber) / 255f;
					g = int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber) / 255f;
					b = int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber) / 255f;
					a = int.Parse(colorcode.Substring(6, 2), NumberStyles.HexNumber) / 255f;
				}
			}
			return new Color(r,g,b,a);
		}

		public override string ToString() {
			return $"Color({r}, {g}, {b}, {a})";
		}
	}
}