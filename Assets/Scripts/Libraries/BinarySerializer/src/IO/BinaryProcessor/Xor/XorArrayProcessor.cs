using System;

namespace BinarySerializer
{
    public class XorArrayProcessor : XorProcessor
    {
        public XorArrayProcessor(byte[] key, int byteIndex = 0, int? maxLength = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ByteIndex = byteIndex;
            MaxLength = maxLength;
        }

        public byte[] Key { get; }
        public int ByteIndex { get; set; }
        public int? MaxLength { get; }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                if (ByteIndex >= MaxLength)
                {
                    ByteIndex++;
                    continue;
                }

                byte key = Key[ByteIndex % Key.Length];
                ByteIndex++;
                buffer[i] = (byte)(buffer[i] ^ key);
            }
        }
    }
}