#nullable enable
using System;

namespace BinarySerializer
{
    public abstract class BitSerializerObject 
    {
        #region Constructor

        protected BitSerializerObject(SerializerObject serializerObject, Pointer valueOffset, string? logPrefix, long value)
        {
            SerializerObject = serializerObject ?? throw new ArgumentNullException(nameof(serializerObject));
            ValueOffset = valueOffset ?? throw new ArgumentNullException(nameof(valueOffset));
            BaseLogPrefix = logPrefix;
            Value = value;
            Position = 0;
        }

        #endregion

        #region Protected Constant Fields

        protected const string DefaultName = "<no name>";

        #endregion

        #region Protected Properties

        protected string? BaseLogPrefix { get; }

        protected string? LogPrefix => SerializerObject.IsSerializerLoggerEnabled 
            ? $"{BaseLogPrefix}{new string(' ', (Depth + 1) * 2)}" 
            : null;

        protected bool DisableSerializerLogForObject { get; set; }

        #endregion

        #region Public Properties

        public SerializerObject SerializerObject { get; }
        public Context Context => SerializerObject.Context;
        public Pointer ValueOffset { get; }
        public long Value { get; set; }
        public int Position { get; set; }

        /// <summary>
        /// The current depth when serializing objects
        /// </summary>
        public int Depth { get; protected set; } = 0;

        #endregion

        #region Serializer Methods

        public abstract T SerializeBits<T>(
            T value, 
            int length, 
            SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned, 
            string? name = null)
            where T : struct;

        public abstract T? SerializeNullableBits<T>(
            T? value, 
            int length, 
            string? name = null)
            where T : struct;

        public void SerializePadding(int length, bool logIfNotNull = false, string? name = "Padding")
        {
            int pos = Position;

            long v = SerializeBits<long>(default, length, name: name);

            if (logIfNotNull && SerializerObject.Defaults?.DisableFormattingWarnings != true && v != 0)
                Context.SystemLogger?.LogWarning("Padding at {0} (bit position {1}) contains data! Data: 0x{2:X8}", ValueOffset, pos, v);
        }

        /// <summary>
        /// Serializes a <see cref="BitSerializable"/> object
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        /// <param name="name">A name can be provided optionally, for logging or text serialization purposes</param>
        /// <returns>The object that was serialized</returns>
        public abstract T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null) 
            where T : BitSerializable, new();

        #endregion
    }
}