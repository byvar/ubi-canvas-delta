#nullable enable
using System;

namespace BinarySerializer 
{
    public class BitSerializer : BitSerializerObject 
    {
        public BitSerializer(SerializerObject serializerObject, Pointer valueOffset, string? logPrefix, long value) 
            : base(serializerObject, valueOffset, logPrefix, value) { }

        public override T SerializeBits<T>(
            T value,
            int length,
            SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned,
            string? name = null)
        {
            long valueToWrite = ObjectToLong(value);
            Value = BitHelpers.SetBits64(Value, valueToWrite, length, Position, sign: sign);

            if (SerializerObject.IsSerializerLoggerEnabled && !DisableSerializerLogForObject)
                Context.SerializerLogger.Log($"{LogPrefix}  {Position}_{length} ({typeof(T).Name}) {name ?? DefaultName}: {valueToWrite}");

            Position += length;

            return value;
        }

        public override T? SerializeNullableBits<T>(T? value, int length, string? name = null)
        {
            long valueToWrite;

            if (value == null)
                valueToWrite = (long)(Math.Pow(2, length) - 1);
            else
                valueToWrite = ObjectToLong(value);

            Value = BitHelpers.SetBits64(Value, valueToWrite, length, Position);

            if (SerializerObject.IsSerializerLoggerEnabled && !DisableSerializerLogForObject)
                Context.SerializerLogger.Log($"{LogPrefix}  {Position}_{length} ({typeof(T).Name}?) {name ?? DefaultName}: {value?.ToString() ?? "null"}");

            Position += length;

            return value;
        }

        public override T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null)
            where T : class
        {
            long pos = Position;

            obj ??= new T();

            // Reinitialize object
            obj.Init(ValueOffset, pos);

            string? logString = LogPrefix;
            bool isLogTemporarilyDisabled = false;

            if (!DisableSerializerLogForObject && obj.UseShortLog) 
            {
                DisableSerializerLogForObject = true;
                isLogTemporarilyDisabled = true;
            }

            if (SerializerObject.IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}{pos} (Object: {typeof(T)}) {name ?? DefaultName}");

            try 
            {
                Depth++;
                onPreSerialize?.Invoke(obj);
                obj.Serialize(this);
            } 
            finally 
            {
                Depth--;

                if (isLogTemporarilyDisabled) 
                {
                    DisableSerializerLogForObject = false;
                
                    if (SerializerObject.IsSerializerLoggerEnabled)
                        Context.SerializerLogger.Log($"{logString}{pos}_{obj.Size} ({typeof(T)}) {name ?? DefaultName}: {obj.ShortLog}");
                }
            }

            return obj;
        }

        protected long ObjectToLong(object value)
        {
            if (value == null) 
                throw new ArgumentNullException(nameof(value));
            
            if (value.GetType().IsEnum)
                return ObjectToLong(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));

            return value switch
            {
                bool v => v ? 1 : 0,
                sbyte v => v,
                byte v => v,
                short v => v,
                ushort v => v,
                int v => v,
                uint v => v,
                long v => v,
                ulong v => BitConverter.ToInt64(BitConverter.GetBytes(v), 0),
                float v => BitConverter.ToInt64(BitConverter.GetBytes(v), 0),
                double v => BitConverter.ToInt64(BitConverter.GetBytes(v), 0),
                UInt24 v => v.Value,
                _ => throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported.")
            };
        }
    }
}