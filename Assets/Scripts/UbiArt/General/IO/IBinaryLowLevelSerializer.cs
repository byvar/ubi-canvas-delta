using System;
using System.IO;
using System.Text;

namespace UbiArt {
	public interface IBinaryLowLevelSerializer : IDisposable {
		public bool IsLittleEndian { get; set; }

		public double SerializeDouble(double value);
		public float SerializeSingle(float value);

		public ulong SerializeUInt64(ulong value);
		public long SerializeInt64(long value);

		public uint SerializeUInt32(uint value);
		public int SerializeInt32(int value);

		public ushort SerializeUInt16(ushort value);
		public short SerializeInt16(short value);

		public byte SerializeUByte(byte value);
		public sbyte SerializeSByte(sbyte value);

		public byte[] SerializeBytes(byte[] bytes, int count);

		public char SerializeChar(char value);

		public string SerializeString8(string value, Encoding encoding = null);
		public string SerializeString16(string value);

		public long Position { get; set; }
		public long Length { get; }
		public Stream BaseStream { get; }
	}
}