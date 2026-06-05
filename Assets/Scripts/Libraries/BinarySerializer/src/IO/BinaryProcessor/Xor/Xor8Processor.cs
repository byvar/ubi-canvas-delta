namespace BinarySerializer
{
    public class Xor8Processor : XorProcessor
    {
        public Xor8Processor(byte xorKey)
        {
            XorKey = xorKey;
        }

        public byte XorKey { get; }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; i++)
                buffer[i] ^= XorKey;
        }
    }
}