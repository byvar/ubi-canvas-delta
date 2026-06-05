#nullable enable
using System;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    [Serializable]
    public class MissingPreValueException : BinarySerializableException
    {
        public MissingPreValueException(BinarySerializable data, string? preValueName) 
            : base(data, $"Missing required pre-value {preValueName}")
        {
            PreValueName = preValueName;
        }
        public MissingPreValueException(BinarySerializable data, string? preValueName, string message) 
            : base(data, message)
        {
            PreValueName = preValueName;
        }

        public MissingPreValueException(BinarySerializable data, string? preValueName, string message, Exception inner)
            : base(data, message, inner)
        {
            PreValueName = preValueName;
        }
        protected MissingPreValueException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public string? PreValueName { get; }
    }
}