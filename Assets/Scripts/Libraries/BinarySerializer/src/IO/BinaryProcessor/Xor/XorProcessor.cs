namespace BinarySerializer
{
    public abstract class XorProcessor : BinaryProcessor
    {
        protected XorProcessor()
        {
            Flags |= BinaryProcessorFlags.ProcessBytes;
        }
    }
}