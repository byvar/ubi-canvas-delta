#nullable enable
namespace BinarySerializer
{
    /// <summary>
    /// An empty serializer log which is always disabled
    /// </summary>
    public class EmptySerializerLogger : ISerializerLogger
    {
        public bool IsEnabled => false;

        public void Log(object? obj) { }
        public void Dispose() { }
    }
}