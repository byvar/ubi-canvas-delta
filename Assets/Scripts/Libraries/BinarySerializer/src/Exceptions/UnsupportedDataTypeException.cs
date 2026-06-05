#nullable enable
using System;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    [Serializable]
    public class UnsupportedDataTypeException : Exception
    {
        public UnsupportedDataTypeException() { }

        public UnsupportedDataTypeException(string? message) : base(message) { }

        public UnsupportedDataTypeException(string? message, Exception inner) : base(message, inner) { }

        protected UnsupportedDataTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}