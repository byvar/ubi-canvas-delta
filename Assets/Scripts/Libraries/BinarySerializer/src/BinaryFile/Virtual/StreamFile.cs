#nullable enable
using System;
using System.IO;

namespace BinarySerializer
{
    /// <summary>
    /// A <see cref="BinaryFile"/> used for a <see cref="System.IO.Stream"/>. This type of file should only be used for limited operations, such as serializing an encoded file.
    /// </summary>
    public class StreamFile : VirtualFile 
    {
        public StreamFile(
            Context context, 
            string name, 
            Stream stream, 
            Endian? endianness = null, 
            bool allowLocalPointers = false, 
            Pointer? parentPointer = null,
            VirtualFileMode mode = VirtualFileMode.Close) 
            : base(context, name, endianness, parentPointer: parentPointer)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Length = stream.Length;
            AllowLocalPointers = allowLocalPointers;
            Mode = mode;

            // Default to current file to avoid issues with pointers being serialized within encoded data
            FileRedirectBehavior = RedirectBehavior.CurrentFile;
        }

        private Stream? _stream;

        public override long Length { get; }
        public override bool IsMemoryMapped => false;

        public bool AllowLocalPointers { get; }
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

        public override BinaryFile? GetPointerFile(long serializedValue, Pointer? anchor = null)
        {
            if (AllowLocalPointers)
                return GetLocalPointerFile(serializedValue, anchor);
            else
                return GetMemoryMappedPointerFile(serializedValue, anchor);
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