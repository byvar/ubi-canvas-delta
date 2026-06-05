#nullable enable
namespace BinarySerializer
{
    public enum VirtualFileMode
    {
        /// <summary>
        /// The default behavior. Disposes and removes a reference to the stream.
        /// </summary>
        Close,

        /// <summary>
        /// Removed a reference to the stream, but does not dispose it.
        /// </summary>
        DoNotClose,

        /// <summary>
        /// Maintains the reference and does not dispose the stream. This allows it to be used multiple times.
        /// </summary>
        Maintain,
    }
}