#nullable enable
using System;
using System.IO;

namespace BinarySerializer
{
    /// <summary>
    /// Extension methods for <see cref="IStreamEncoder"/>
    /// </summary>
    public static class StreamEncoderExtensions
    {
        /// <summary>
        /// Decodes data from a byte array
        /// </summary>
        /// <param name="encoder">The encoder</param>
        /// <param name="data">The data to decode</param>
        /// <returns>The decoded data</returns>
        public static byte[] DecodeBuffer(this IStreamEncoder encoder, byte[] data)
        {
            if (encoder == null) 
                throw new ArgumentNullException(nameof(encoder));
            if (data == null) 
                throw new ArgumentNullException(nameof(data));

            // Create memory streams
            using MemoryStream inputStream = new(data);
            using MemoryStream outputStream = new();

            // Decode the data
            encoder.DecodeStream(inputStream, outputStream);

            // Return the output
            return outputStream.ToArray();
        }

        /// <summary>
        /// Encodes data from a byte array
        /// </summary>
        /// <param name="encoder">The encoder</param>
        /// <param name="data">The data to encode</param>
        /// <returns>The encoded data</returns>
        public static byte[] EncodeBuffer(this IStreamEncoder encoder, byte[] data)
        {
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Create memory streams
            using MemoryStream inputStream = new(data);
            using MemoryStream outputStream = new();

            // Encode the data
            encoder.EncodeStream(inputStream, outputStream);

            // Return the output
            return outputStream.ToArray();
        }
    }
}