#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerializer 
{
    // TODO: Use value buffer like in the Reader to avoid allocating a new byte array on each write call

    public class Writer : BinaryWriter 
    {
        #region Constructors

        public Writer(Stream stream, bool isLittleEndian = true, bool leaveOpen = false) 
            : base(new StreamWrapper(stream), new UTF8Encoding(), leaveOpen)
        {
            IsLittleEndian = isLittleEndian;
        }

        #endregion

        #region Public Properties

        public bool IsLittleEndian { get; set; }
        public new StreamWrapper BaseStream => (StreamWrapper)base.BaseStream;

        #endregion

        #region Write Methods

        public override void Write(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(ulong value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public void Write(UInt24 value)
        {
            uint v = (uint)value;
            if (IsLittleEndian)
            {
                Write((byte)(v & 0xFF));
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)((v >> 16) & 0xFF));
            }
            else
            {
                Write((byte)((v >> 16) & 0xFF));
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)(v & 0xFF));
            }
        }

        public override void Write(float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public override void Write(double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian) 
                Array.Reverse(data);
            Write(data);
        }

        public void WriteNullDelimitedString(string? value, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            value ??= String.Empty;
            byte[] data = encoding.GetBytes(value + '\0');
            Write(data);
        }

        public void WriteString(string? value, long size, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            value ??= String.Empty;
            byte[] data = encoding.GetBytes(value);
            if (data.Length != size)
                Array.Resize(ref data, (int)size);
            Write(data);
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