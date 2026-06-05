#nullable enable
using System;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    [Serializable]
    public class UnsupportedFormatVersionException : BinarySerializableException
    {
        public UnsupportedFormatVersionException(BinarySerializable data) : base(data) { }
        public UnsupportedFormatVersionException(BinarySerializable data, string? message) : base(data, message) { }
        public UnsupportedFormatVersionException(BinarySerializable data, string? message, Exception inner) : base(data, message, inner) { }
        protected UnsupportedFormatVersionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}