#nullable enable
using System;
using System.IO;

namespace BinarySerializer
{
    /// <summary>
    /// An encoder wrapper for <see cref="BinaryProcessor"/>. It is recommended using
    /// <see cref="SerializerObject.DoProcessed"/> where possible rather than this.
    /// </summary>
    public class ProcessorEncoder : IStreamEncoder
    {
        public ProcessorEncoder(BinaryProcessor binaryProcessor, long length)
        {
            BinaryProcessor = binaryProcessor ?? throw new ArgumentNullException(nameof(binaryProcessor));
            Length = length;
            Name = binaryProcessor.GetType().Name;
        }

        public BinaryProcessor BinaryProcessor { get; }
        public long Length { get; }
        public string Name { get; }

        public void DecodeStream(Stream input, Stream output)
        {
            if (input == null) 
                throw new ArgumentNullException(nameof(input));
            if (output == null) 
                throw new ArgumentNullException(nameof(output));
            
            byte[] buffer = new byte[Length];
            input.Read(buffer, 0, buffer.Length);

            if ((BinaryProcessor.Flags & BinaryProcessorFlags.ProcessBytes) != 0)
                BinaryProcessor.ProcessBytes(buffer, 0, buffer.Length);

            output.Write(buffer, 0, buffer.Length);
        }

        public void EncodeStream(Stream input, Stream output) => DecodeStream(input, output);
    }
}