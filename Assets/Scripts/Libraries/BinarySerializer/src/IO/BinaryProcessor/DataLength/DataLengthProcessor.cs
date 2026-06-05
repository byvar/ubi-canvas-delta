namespace BinarySerializer
{
    public class DataLengthProcessor : CalculatedValueProcessor
    {
        public DataLengthProcessor()
        {
            Flags |= BinaryProcessorFlags.ProcessBytes;
        }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            CalculatedValue += count;
        }
    }
}