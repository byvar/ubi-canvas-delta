#nullable enable
using System;
using System.IO;

namespace BinarySerializer
{
    public class MemoryMappedStreamFile : VirtualFile
    {
        public MemoryMappedStreamFile(
            Context context, 
            string name, 
            long baseAddress, 
            Stream stream, 
            Endian? endianness = null,
            long memoryMappedPriority = -1, 
            Pointer? parentPointer = null,
            VirtualFileMode mode = VirtualFileMode.Close) 
            : base(context, name, endianness, baseAddress, memoryMappedPriority: memoryMappedPriority, parentPointer: parentPointer)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Length = stream.Length;
            Mode = mode;
        }

        public MemoryMappedStreamFile(
            Context context, 
            string name, 
            long baseAddress, 
            byte[] buffer, 
            Endian? endianness = null,
            long memoryMappedPriority = -1, 
            Pointer? parentPointer = null,
            VirtualFileMode mode = VirtualFileMode.Close) 
            : this(context, name, baseAddress, new MemoryStream(buffer), endianness, memoryMappedPriority, parentPointer, mode)
        { }

        private Stream? _stream;

        public override long Length { get; }
        public override bool IsMemoryMapped => true;

        public VirtualFileMode Mode { get; }

        protected Stream Stream
        {
            get => _stream ?? throw new ObjectDisposedException(nameof(Stream));
            set => _stream = value;
        }

        public override Reader CreateReader()
        {
            Reader reader = new(Stream, isLittleEndian: Endianness == Endian.Little, leaveOpen: Mode != VirtualFileMode.Close);
            return reader;
        }

        public override Writer CreateWriter()
        {
            Writer writer = new(Stream, isLittleEndian: Endianness == Endian.Little, leaveOpen: Mode != VirtualFileMode.Close);
            Stream.Position = 0;
            return writer;
        }

        public override void Dispose()
        {
            // Dispose base file
            base.Dispose();

            if (Mode == VirtualFileMode.Maintain)
                return;

            if (Mode == VirtualFileMode.Close)
                _stream?.Dispose();

            _stream = null;
        }
    }
}