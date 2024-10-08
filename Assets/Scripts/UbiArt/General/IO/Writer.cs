﻿using System;
using System.IO;
using System.Text;

namespace UbiArt {
    public class Writer : BinaryWriter, IBinaryLowLevelSerializer {
		#region Constructors

		public Writer(Stream stream, bool isLittleEndian = true, bool leaveOpen = false) : base(stream, new UTF8Encoding(), leaveOpen) {
			IsLittleEndian = isLittleEndian;
		}

		#endregion

		#region Public Properties

		public bool IsLittleEndian { get; set; }

		public long Position {
			get => BaseStream.Position;
			set => BaseStream.Position = value;
		}
		public long Length => BaseStream.Length;
		#endregion

		#region Protected Properties

		protected uint BytesSinceAlignStart { get; set; }
        protected bool AutoAlignOn { get; set; }

		#endregion

		#region Write Methods

		public override void Write(int value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(short value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(uint value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(ushort value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(long value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(ulong value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(float value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(double value) {
            var data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public void WriteNullDelimitedString(string value, Encoding encoding) {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (value == null) value = "";
            byte[] data = encoding.GetBytes(value + '\0');
            Write(data);
        }

        public void WriteString(string value, long size, Encoding encoding) {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            value ??= "";
            byte[] data = encoding.GetBytes(value + '\0');
            if (data.Length != size)
                Array.Resize(ref data, (int)size);
            Write(data);
        }

        public override void Write(byte[] buffer) {
            if (buffer == null)
                return;

            base.Write(buffer);

            if (AutoAlignOn)
                BytesSinceAlignStart += (uint)buffer.Length;
        }

        public override void Write(byte value) {
            base.Write(value);

            if (AutoAlignOn)
                BytesSinceAlignStart++;
        }

        public override void Write(sbyte value) => Write((byte)value);

        public override void Write(Char value) {
            Write(Convert.ToByte(value));
        }

        public override void Write(bool value) {
            if (value) Write((int)1);
            else Write((int)0);
        }
		public override void Write(string value) {
			throw new NotImplementedException($"Use Write8 or Write16");

		}
		public void Write8(String value, Encoding encoding = null) {
            if (value == null) value = "";
			encoding ??= Encoding.UTF8;
			var data = encoding.GetBytes(value);
            Write(data.Length);
            Write(data);
        }
        public void Write16(String value) {
            if (value == null) value = "";
            var data = Encoding.BigEndianUnicode.GetBytes(value);
            Write(data.Length / 2);
            Write(data);
        }
        #endregion

        #region Alignment

        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                int length = alignBytes - (int)(BaseStream.Position % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
        }
        public void AlignOffset(int alignBytes, int offset) {
            if ((BaseStream.Position - offset) % alignBytes != 0) {
                int length = alignBytes - (int)((BaseStream.Position - offset) % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                int length = alignBytes - rest;
                byte[] data = new byte[length];
                Write(data);
            }
        }

        public void AutoAlign(int alignBytes) {
            if (BytesSinceAlignStart % alignBytes != 0) {
                int length = alignBytes - (int)(BytesSinceAlignStart % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
            BytesSinceAlignStart = 0;
        }

		#endregion

		#region Low Level Serializer
		public uint SerializeUInt32(uint value) {
			Write(value);
			return value;
		}

		public int SerializeInt32(int value) {
			Write(value);
			return value;
		}

		public ushort SerializeUInt16(ushort value) {
			Write(value);
			return value;
		}

		public short SerializeInt16(short value) {
			Write(value);
			return value;
		}

		public byte SerializeUByte(byte value) {
			Write(value);
			return value;
		}

		public sbyte SerializeSByte(sbyte value) {
			Write(value);
			return value;
		}

		public byte[] SerializeBytes(byte[] bytes, int count) {
			Write(bytes);
			return bytes;
		}

		public char SerializeChar(char value) {
			Write(value);
			return value;
		}

		public string SerializeString8(string value, Encoding encoding = null) {
			Write8(value, encoding: encoding);
			return value;
		}

		public string SerializeString16(string value) {
			Write16(value);
			return value;
		}

		public double SerializeDouble(double value) {
			Write(value);
			return value;
		}

		public float SerializeSingle(float value) {
			Write(value);
			return value;
		}

		public ulong SerializeUInt64(ulong value) {
			Write(value);
			return value;
		}

		public long SerializeInt64(long value) {
			Write(value);
			return value;
		}
		#endregion
	}
}