#nullable enable
using System;

namespace BinarySerializer
{
    /// <summary>
    /// A binary processor which calculates a value which is then serialized
    /// </summary>
    public abstract class CalculatedValueProcessor : BinaryProcessor
    {
        protected CalculatedValueProcessor()
        {
            Flags |= BinaryProcessorFlags.Callbacks;
        }

        private bool _hasEnded;
        private bool _hasSerialized;

        private Pointer? _serializePointer;
        private Type? _serializeType;
        private SerializerObject? _serializerObject;
        private string? _serializeName;

        /// <summary>
        /// The calculated value
        /// </summary>
        public virtual long CalculatedValue { get; set; }
        public long SerializedValue { get; set; }

        private void SkipValue(Type type, SerializerObject s, string? name)
        {
            if (type == typeof(byte) || type == typeof(sbyte))
                s.Goto(s.CurrentPointer + 1);
            else if (type == typeof(ushort) || type == typeof(short))
                s.Goto(s.CurrentPointer + 2);
            else if (type == typeof(UInt24))
                s.Goto(s.CurrentPointer + 3);
            else if (type == typeof(uint) || type == typeof(int))
                s.Goto(s.CurrentPointer + 4);
            else if (type == typeof(ulong) || type == typeof(long))
                s.Goto(s.CurrentPointer + 8);
            else
                throw new NotSupportedException($"The specified value type {type} for {name} can not be serialized as an integer");
        }

        private void SerializeValue(Type type, SerializerObject s, string? name)
        {
            // The value itself shouldn't be processed
            DoInactive(() =>
            {
                if (type == typeof(byte))
                    SerializedValue = s.Serialize<byte>((byte)CalculatedValue, name: name);
                else if (type == typeof(sbyte))
                    SerializedValue = s.Serialize<sbyte>((sbyte)CalculatedValue, name: name);
                else if (type == typeof(ushort))
                    SerializedValue = s.Serialize<ushort>((ushort)CalculatedValue, name: name);
                else if (type == typeof(short))
                    SerializedValue = s.Serialize<short>((short)CalculatedValue, name: name);
                else if (type == typeof(UInt24))
                    SerializedValue = s.Serialize<UInt24>((UInt24)CalculatedValue, name: name);
                else if (type == typeof(uint))
                    SerializedValue = s.Serialize<uint>((uint)CalculatedValue, name: name);
                else if (type == typeof(int))
                    SerializedValue = s.Serialize<int>((int)CalculatedValue, name: name);
                else if (type == typeof(ulong))
                    SerializedValue = (long)s.Serialize<ulong>((ulong)CalculatedValue, name: name);
                else if (type == typeof(long))
                    SerializedValue = s.Serialize<long>((long)CalculatedValue, name: name);
                else
                    throw new NotSupportedException($"The specified value type {type} for {name} can not be serialized as an integer");

                _hasSerialized = true;
            });
        }

        private void VerifyValue(SerializerObject s)
        {
            if (CalculatedValue != SerializedValue)
            {
                s.SystemLogger?.LogWarning("{0}: Calculated value {1} does not match serialized value {2}!", GetType().Name, CalculatedValue, SerializedValue);

                s.Log("{0}: Calculated value {1} does not match serialized value {2}!", GetType().Name, CalculatedValue, SerializedValue);
            }
        }

        public override void EndProcessing(SerializerObject s)
        {
            base.EndProcessing(s);

            _hasEnded = true;

            // Serialize postponed serialization and verify it
            if (_serializePointer != null && _serializeType != null && _serializerObject != null)
            {
                s.DoAt(_serializePointer, () => SerializeValue(_serializeType, _serializerObject, _serializeName));
                VerifyValue(s);

                _serializePointer = null;
                _serializeType = null;
                _serializerObject = null;
            }
            // If we've already serialized the value we want to verify it
            else if (_hasSerialized)
            {
                VerifyValue(s);
            }
        }

        public void Serialize<T>(SerializerObject s, string? name = null)
        {
            // If the processing has ended we serialize the value directly and then verify it
            if (_hasEnded)
            {
                SerializeValue(typeof(T), s, name);
                VerifyValue(s);
            }
            // If we're deserializing we can optimize it by serializing the value directly. It
            // will then verify it when the processing has ended.
            else if (s is BinaryDeserializer)
            {
                // Serialize directly
                SerializeValue(typeof(T), s, name);
            }
            // Otherwise postpone serializing until the value has been fully calculated
            else
            {
                _serializePointer = s.CurrentPointer;
                _serializeType = typeof(T);
                _serializerObject = s;
                _serializeName = name;

                SkipValue(typeof(T), s, name);
            }
        }
    }
}