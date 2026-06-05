#nullable enable
using System;

namespace BinarySerializer
{
    /// <summary>
    /// A binary processor for processing binary data when serializing it. This differs from an
    /// encoder in the sense that it processes the data as it's being serialized and doesn't
    /// change its length.
    /// </summary>
    public abstract class BinaryProcessor
    {
        public BinaryProcessorFlags Flags { get; protected set; }
        public bool IsActive { get; set; } = true;

        public virtual void BeginProcessing(SerializerObject s) { }
        public virtual void EndProcessing(SerializerObject s) { }
        public virtual void ProcessBytes(byte[] buffer, int offset, int count) { }

        public void DoInactive(Action action)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            bool isActive = IsActive;

            IsActive = false;

            try
            {
                action();
            }
            finally
            {
                IsActive = isActive;
            }
        }
    }
}