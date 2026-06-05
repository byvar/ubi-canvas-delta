#nullable enable
namespace BinarySerializer
{
    /// <summary>
    /// A <see cref="BinaryFile"/> which uses a virtual file
    /// </summary>
    public abstract class VirtualFile : BinaryFile
    {
        protected VirtualFile(
            Context context, 
            string filePath, 
            Endian? endianness = null, 
            long baseAddress = 0, 
            Pointer? startPointer = null, 
            long memoryMappedPriority = -1, 
            Pointer? parentPointer = null) 
            : base(context, filePath, endianness, baseAddress, startPointer, memoryMappedPriority)
        {
            ParentPointer = parentPointer;
        }

        /// <summary>
        /// An optional pointer to the start of this file in the parent file
        /// </summary>
        public Pointer? ParentPointer { get; }
    }
}