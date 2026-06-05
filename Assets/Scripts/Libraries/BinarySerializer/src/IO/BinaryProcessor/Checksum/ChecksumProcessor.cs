namespace BinarySerializer
{
    public abstract class ChecksumProcessor : CalculatedValueProcessor
    {
        protected ChecksumProcessor()
        {
            Flags |= BinaryProcessorFlags.ProcessBytes;
        }
    }
}