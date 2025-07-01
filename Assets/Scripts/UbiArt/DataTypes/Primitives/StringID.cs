using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace UbiArt {
	public class StringID : ICSerializable, IEquatable<StringID>, ICSerializableShortLog {
		public uint stringID;

		public void Serialize(CSerializerObject s, string name) {
			stringID = s.Serialize<uint>(stringID);
		}

		public bool IsNull {
			get {
				return stringID == 0xFFFFFFFF;
			}
		}

		public StringID() {
			stringID = 0xFFFFFFFF;
		}
		public StringID(uint stringID) {
			this.stringID = stringID;
		}

		public StringID(string str) : this(str != null ? ASCIIEncoding.ASCII.GetBytes(str) : null) {}
		public StringID(byte[] array, uint? length = null) {
			if (array == null) {
				stringID = 0xFFFFFFFF;
				return;
			}
			if (!length.HasValue) {
				length = (uint)array.Length;
			}


			// Jenkins lookup3 hash but with different init values
			const uint InitVal = 0x9E3779B9; // Golden ratio
			uint a = InitVal, b = InitVal, c = 0;
			uint pos = 0;

			// Reuse the same array for performance
			byte[] upper = array;

			// StringID is generated for uppercase strings only
			for (int i = 0; i < length; i++) {
				upper[i] = ToUp(array[i]);
			}

			// Process 12-byte blocks
			while (pos + 12 <= length) {
				a += upper[pos] | (uint)(upper[pos + 1] << 8) | (uint)(upper[pos + 2] << 16) | (uint)(upper[pos + 3] << 24);
				b += upper[pos + 4] | (uint)(upper[pos + 5] << 8) | (uint)(upper[pos + 6] << 16) | (uint)(upper[pos + 7] << 24);
				c += upper[pos + 8] | (uint)(upper[pos + 9] << 8) | (uint)(upper[pos + 10] << 16) | (uint)(upper[pos + 11] << 24);

				Mix(ref a, ref b, ref c);
				pos += 12;
			}

			// Remaining bytes
			c += length.Value;
			switch (length - pos) {
				case 11: c += (uint)upper[pos + 10] << 24; goto case 10;
				case 10: c += (uint)upper[pos + 9] << 16; goto case 9;
				case 9: c += (uint)upper[pos + 8] << 8; goto case 8;
				case 8: b += (uint)upper[pos + 7] << 24; goto case 7;
				case 7: b += (uint)upper[pos + 6] << 16; goto case 6;
				case 6: b += (uint)upper[pos + 5] << 8; goto case 5;
				case 5: b += upper[pos + 4]; goto case 4;
				case 4: a += (uint)upper[pos + 3] << 24; goto case 3;
				case 3: a += (uint)upper[pos + 2] << 16; goto case 2;
				case 2: a += (uint)upper[pos + 1] << 8; goto case 1;
				case 1: a += upper[pos + 0]; break;
			}

			Mix(ref a, ref b, ref c);
			stringID = c;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static byte ToUp(byte c) {
			return (c >= 'a' && c <= 'z') ? (byte)(c - 32) : c;
		}
		// Jenkins lookup3 mix function
		private static void Mix(ref uint a, ref uint b, ref uint c) {
			a -= b; a -= c; a ^= (c >> 13);
			b -= c; b -= a; b ^= (a << 8);
			c -= a; c -= b; c ^= (b >> 13);
			a -= b; a -= c; a ^= (c >> 12);
			b -= c; b -= a; b ^= (a << 16);
			c -= a; c -= b; c ^= (b >> 5);
			a -= b; a -= c; a ^= (c >> 3);
			b -= c; b -= a; b ^= (a << 10);
			c -= a; c -= b; c ^= (b >> 15);
		}
		public static implicit operator StringID(string s) {
			return new StringID(s);
		}

		public override string ToString() => $"StringID(0x{stringID.ToString("X8")})";

		public string ToString(Context c, bool shortString = false) {
			string str = ToString();
			if (!IsNull && c != null && c.StringCache.ContainsKey(this)) {
				if (shortString) {
					str = c.StringCache[this].Replace("\n", "\\n");
				} else {
					str += " - " + c.StringCache[this].Replace("\n", "\\n");
				}
			}
			return str;
		}

		public override bool Equals(object obj) {
			return obj is StringID && this == (StringID)obj;
		}
		public override int GetHashCode() {
			return stringID.GetHashCode();
		}

		public bool Equals(StringID other) {
			return this == (StringID)other;
		}

		public string SerializeLog(CSerializerObject s) => ToString(s?.Context);

		public static bool operator ==(StringID x, StringID y) {
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			return x.stringID == y.stringID;
		}
		public static bool operator !=(StringID x, StringID y) {
			return !(x == y);
		}
	}
}
