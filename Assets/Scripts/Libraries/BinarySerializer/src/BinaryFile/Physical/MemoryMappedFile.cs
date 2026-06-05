#nullable enable
namespace BinarySerializer
{
    public class MemoryMappedFile : PhysicalFile 
    {
        public MemoryMappedFile(
            Context context, 
            string filePath, 
            long baseAddress, 
            Endian? endianness = null, 
            long? fileLength = null, 
            long memoryMappedPriority = -1) 
            : base(context, filePath, endianness, baseAddress, fileLength: fileLength, memoryMappedPriority: memoryMappedPriority) { }

        public override bool IsMemoryMapped => true;
    }
}