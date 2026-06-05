#nullable enable
using System;
using System.IO;

namespace BinarySerializer
{
    public class EncodedLinearFile : PhysicalFile
    {
        public EncodedLinearFile(
            Context context, 
            string filePath, 
            IStreamEncoder encoder, 
            Endian? endianness = null, 
            long? fileLength = null) 
            : base(context, filePath, endianness, fileLength: fileLength)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public IStreamEncoder Encoder { get; }

        public override long Length
        {
            get
            {
                if (FileLength == null)
                {
                    // Open the file
                    using Stream s = FileManager.GetFileReadStream(SourcePath);

                    using var decoded = new MemoryStream();

                    // Decode the file
                    Encoder.DecodeStream(s, decoded);

                    // Set the length
                    FileLength = decoded.Length;
                }

                return FileLength.Value;
            }
        }

        public override bool IsMemoryMapped => false;

        public override Reader CreateReader() 
        {
            // Open the file
            using Stream s = FileManager.GetFileReadStream(SourcePath);

            var decoded = new MemoryStream();

            // Decode the file
            Encoder.DecodeStream(s, decoded);

            decoded.Position = 0;

            // Set the length
            FileLength = decoded.Length;

            // Return a reader
            return new Reader(decoded, isLittleEndian: Endianness == Endian.Little);
        }

        public override Writer CreateWriter() 
        {
            Stream memStream = new MemoryStream();
            Writer writer = new(memStream, isLittleEndian: Endianness == Endian.Little);
            return writer;
        }

        public override void EndWrite(Writer? writer) 
        {
            if (writer != null) 
            {
                CreateBackupFile();

                using Stream s = FileManager.GetFileWriteStream(DestinationPath, RecreateOnWrite);
                writer.BaseStream.Position = 0;
                Encoder.EncodeStream(writer.BaseStream, s);
            }

            base.EndWrite(writer);
        }
    }
}