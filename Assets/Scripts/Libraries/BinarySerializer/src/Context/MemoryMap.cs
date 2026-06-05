#nullable enable
using System.Collections.Generic;

namespace BinarySerializer
{
    public class MemoryMap 
    {
        public MemoryMap()
        {
            Files = new List<BinaryFile>();
            Pointers = new List<Pointer>();
        }

        public List<BinaryFile> Files { get; }

        /// <summary>
        /// Pointers that can be relocated later
        /// </summary>
        public List<Pointer> Pointers { get; }

        /// <summary>
        /// Add a pointer to possibly relocate later
        /// </summary>
        /// <param name="pointer">Pointer to add to list of relocated objects</param>
        public void AddPointer(Pointer? pointer)
        {
            if (pointer == null) 
                return;
            
            Pointers.Add(pointer);
        }

        public void ClearPointers() => Pointers.Clear();
    }
}