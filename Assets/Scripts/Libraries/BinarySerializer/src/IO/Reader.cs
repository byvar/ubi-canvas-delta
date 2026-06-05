#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BinarySerializer
{
    /// <summary>
    /// An extended version of the <see cref="BinaryReader"/> for reading binary data
    /// </summary>
    public class Reader : BinaryReader
    {
        #region Constructors

        public Reader(Stream stream, bool isLittleEndian = true, bool leaveOpen = false) 
            // Wrap the stream to be read in a StreamWrapper so that we can easily process the bytes which get read
            // The encoding passed in to the base ctor is irrelevant since we have re-implemented the string reading
            : base(new StreamWrapper(stream), new UTF8Encoding(), leaveOpen)
        {
            IsLittleEndian = isLittleEndian;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// A common buffer to use for reading value types. This is created once to avoid allocating a new
        /// byte array on each read call. The length is set to 8 due to 64-bit values currently being the largest supported.
        /// </summary>
        protected byte[] ValueBuffer { get; } = new byte[8];

        protected bool RequiresByteReversing => IsLittleEndian != BitConverter.IsLittleEndian;

        #endregion

        #region Public Properties

        public bool IsLittleEndian { get; set; }
        public new StreamWrapper BaseStream => (StreamWrapper)base.BaseStream;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads the specified number of bytes to <see cref="ValueBuffer"/> and reverses them if needed.
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ReadToValueBuffer(int count)
        {
            ReadBytes(ValueBuffer, 0, count);

            if (RequiresByteReversing)
                Array.Reverse(ValueBuffer, 0, count);
        }

        #endregion

        #region Read Methods

        public override int ReadInt32()
        {
            ReadToValueBuffer(4);
            return BitConverter.ToInt32(ValueBuffer, 0);
        }

        public override float ReadSingle()
        {
            ReadToValueBuffer(4);
            return BitConverter.ToSingle(ValueBuffer, 0);
        }

        public override short ReadInt16()
        {
            ReadToValueBuffer(2);
            return BitConverter.ToInt16(ValueBuffer, 0);
        }

        public override ushort ReadUInt16()
        {
            ReadToValueBuffer(2);
            return BitConverter.ToUInt16(ValueBuffer, 0);
        }

        public override long ReadInt64()
        {
            ReadToValueBuffer(8);
            return BitConverter.ToInt64(ValueBuffer, 0);
        }

        public override uint ReadUInt32()
        {
            ReadToValueBuffer(4);
            return BitConverter.ToUInt32(ValueBuffer, 0);
        }

        public override ulong ReadUInt64()
        {
            ReadToValueBuffer(8);
            return BitConverter.ToUInt64(ValueBuffer, 0);
        }

        public UInt24 ReadUInt24()
        {
            var b1 = ReadByte();
            var b2 = ReadByte();
            var b3 = ReadByte();
            if (IsLittleEndian)
            {
                return
                    (UInt24)((((uint)b3) << 16) |
                    (((uint)b2) << 8) |
                    ((uint)b1));
            }
            else
            {
                return
                    (UInt24)((((uint)b1) << 16) |
                    (((uint)b2) << 8) |
                    ((uint)b3));
            }
        }

        public override byte[] ReadBytes(int count)
        {
            if (count == 0)
                return Array.Empty<byte>();

            byte[] buffer = new byte[count];

            ReadBytes(buffer, 0, count, throwOnIncompleteRead: true);

            return buffer;
        }

        public void ReadBytes(byte[] buffer, int offset, int count, bool throwOnIncompleteRead = true)
        {
            if (buffer == null) 
                throw new ArgumentNullException(nameof(buffer));
            if (count < 0) 
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative amount of bytes is required");

            int numRead = 0;
            int toRead = count;
            do
            {
                int n = BaseStream.Read(buffer, offset + numRead, toRead);
                if (n == 0)
                    break;
                numRead += n;
                toRead -= n;
            } while (toRead > 0);

            if (throwOnIncompleteRead && numRead != count)
                throw new EndOfStreamException();
        }

        public string ReadNullDelimitedString(Encoding encoding)
        {
            List<byte> bytes = new();
            byte b = ReadByte();

            while (b != 0x0)
            {
                bytes.Add(b);
                b = ReadByte();
            }

            if (bytes.Count > 0)
            {
                if (encoding == null) 
                    throw new ArgumentNullException(nameof(encoding));
                return encoding.GetString(bytes.ToArray());
            }

            return String.Empty;
        }

        public string ReadString(long size, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            // Read the bytes
            byte[] bytes = ReadBytes((int)size);

            // Get the string from the bytes using the specified encoding
            string str = encoding.GetString(bytes);

            // Trim after first null character
            int nullIndex = str.IndexOf((char)0x00);

            if (nullIndex != -1)
                str = str.Substring(0, nullIndex);

            // Return the string
            return str;
        }

        #endregion

        #region Alignment

        public void Align(long alignBytes)
        {
            long align = BaseStream.Position % alignBytes;

            if (align != 0)
                BaseStream.Position += alignBytes - align;
        }
        public void Align(long alignBytes, long offset)
        {
            long align = (BaseStream.Position - offset) % alignBytes;

            if (align != 0)
                BaseStream.Position += alignBytes - align;
        }

        #endregion

        #region Processors

        public void AddBinaryProcessor(BinaryProcessor binaryProcessor) =>
            BaseStream.BinaryProcessors.Add(binaryProcessor);
        public void RemoveBinaryProcessor(BinaryProcessor binaryProcessor) =>
            BaseStream.BinaryProcessors.Remove(binaryProcessor);
        public T? GetBinaryProcessor<T>()
            where T : BinaryProcessor => 
            BaseStream.BinaryProcessors.OfType<T>().FirstOrDefault();

        #endregion
    }
}