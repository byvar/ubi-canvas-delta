#nullable enable
using System;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    [Serializable]
    public class BinarySerializableException : Exception
    {
        public BinarySerializableException(BinarySerializable? data) 
            : base(FormatMessage(data, null)) { }

        public BinarySerializableException(BinarySerializable? data, string? message) 
            : base(FormatMessage(data, message)) { }

        public BinarySerializableException(BinarySerializable? data, string? message, Exception inner) 
            : base(FormatMessage(data, message), inner) { }

        protected BinarySerializableException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        protected static string FormatMessage(BinarySerializable? data, string? message) => 
            $"{data?.Offset}{(message == null || data == null ? message : $": {message}")}";
    }
}