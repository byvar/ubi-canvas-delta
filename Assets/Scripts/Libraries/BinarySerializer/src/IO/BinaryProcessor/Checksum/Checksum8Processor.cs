namespace BinarySerializer
{
    public class Checksum8Processor : ChecksumProcessor
    {
        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; i++)
                CalculatedValue = (CalculatedValue + buffer[i]) % 256;
        }
    }
}