#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{
    /// <summary>
    /// A base binary serializer used for serializing/deserializing
    /// </summary>
    public abstract class SerializerObject
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The serializer context</param>
        protected SerializerObject(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Protected Constant Fields

        protected const string DefaultName = "<no name>";

        #endregion

        #region Protected Properties

        protected bool DisableSerializerLogForObject { get; set; }
        protected IFileManager FileManager => Context.FileManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The serialize context, containing all the open files and the settings
        /// </summary>
        public Context Context { get; }

        public ISystemLogger? SystemLogger => Context.SystemLogger;

        /// <summary>
        /// The current depth when serializing objects
        /// </summary>
        public int Depth { get; protected set; } = 0;

        /// <summary>
        /// The current length of the data being serialized
        /// </summary>
        public abstract long CurrentLength { get; }

        /// <summary>
        /// The current length as a 32-bit value
        /// </summary>
        public uint CurrentLength32 => (uint)CurrentLength;

        /// <summary>
        /// Indicates if the serializer has a current pointer. If one does not exist then most serialize calls
        /// will result in an exception. Avoid this by calling <see cref="Goto"/> to set a current pointer.
        /// </summary>
        public abstract bool HasCurrentPointer { get; }

        /// <summary>
        /// The current binary file being used by the serializer
        /// </summary>
        public abstract BinaryFile CurrentBinaryFile { get; }

        /// <summary>
        /// The current pointer
        /// </summary>
        public virtual Pointer CurrentPointer => 
            new(CurrentAbsoluteOffset, CurrentBinaryFile, size: Context.Settings.LoggingPointerSize ?? CurrentBinaryFile.PointerSize);

        /// <summary>
        /// The current absolute offset
        /// </summary>
        public virtual long CurrentAbsoluteOffset => CurrentFileOffset + CurrentBinaryFile.BaseAddress;
        
        /// <summary>
        /// The current file offset
        /// </summary>
        public abstract long CurrentFileOffset { get; }

        /// <summary>
        /// Default values for some serializer functions
        /// </summary>
        public virtual SerializerDefaults? Defaults { get; set; }

        /// <summary>
        /// Indicates if the serialization names are being used by this serializer object
        /// </summary>
        public abstract bool UsesSerializeNames { get; }

        // TODO: Remove this and replace with a better system for determining if data referenced from pointers should be serialized
        public virtual bool FullSerialize => true;

        #endregion

        #region Serializer Logging

        /// <summary>
        /// Indicates if logging is enabled for the serialization
        /// </summary>
        public bool IsSerializerLoggerEnabled => Context.SerializerLogger.IsEnabled && !DisableSerializerLogForObject;

        /// <summary>
        /// Writes a line to the serializer log, if enabled
        /// </summary>
        /// <param name="logString">The string to log</param>
        /// <param name="args">The log string arguments</param>
        [JetBrains.Annotations.StringFormatMethod("logString")]
        public abstract void Log(string logString, params object[] args);

        #endregion

        #region Encoding

        private List<EncodedState>? _encodedFiles;
        protected List<EncodedState> EncodedFiles => _encodedFiles ??= new List<EncodedState>();

        protected class EncodedState
        {
            public EncodedState(Stream stream, StreamFile file, IStreamEncoder encoder)
            {
                Stream = stream ?? throw new ArgumentNullException(nameof(stream));
                File = file ?? throw new ArgumentNullException(nameof(file));
                Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            }

            public Stream Stream { get; }
            public StreamFile File { get; }
            public IStreamEncoder Encoder { get; }
        }

        public abstract Pointer BeginEncoded(
            IStreamEncoder encoder, 
            Endian? endianness = null, 
            bool allowLocalPointers = false, 
            string? filename = null);
        public abstract void EndEncoded(Pointer endPointer);
        public abstract void DoEncoded(
            IStreamEncoder? encoder, 
            Action action, 
            Endian? endianness = null, 
            bool allowLocalPointers = false, 
            string? filename = null);

        #endregion

        #region Processing

        public abstract void BeginProcessed(BinaryProcessor processor);
        public abstract void EndProcessed(BinaryProcessor processor);

        public T? DoProcessed<T>(T? processor, Action action)
            where T : BinaryProcessor
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            if (processor == null)
            {
                action();
                return null;
            }

            try
            {
                BeginProcessed(processor);
                action();
            }
            finally
            {
                EndProcessed(processor);
            }

            return processor;
        }
        public T? DoProcessed<T>(T? processor, Action<T?> action)
            where T : BinaryProcessor
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            if (processor == null)
            {
                action(null);
                return null;
            }

            try
            {
                BeginProcessed(processor);
                action(processor);
            }
            finally
            {
                EndProcessed(processor);
            }

            return processor;
        }

        public abstract T? GetProcessor<T>()
            where T : BinaryProcessor;

        #endregion

        #region Positioning

        public abstract void Goto(Pointer? offset);

        public abstract void Align(int alignBytes = 4, Pointer? baseOffset = null, bool? logIfNotNull = null);

        public virtual void DoAt(Pointer? offset, Action action)
        {
            if (offset == null) 
                return;

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Pointer? currentOffset = HasCurrentPointer ? CurrentPointer : null;
            Goto(offset);

            try
            {
                action();
            }
            finally
            {
                Goto(currentOffset);
            }
        }

        // TODO: Remove this? This overload causes issues for the size serializer where DoAt is disabled thus causing this to
        //       always return null. A potential fix is to allow for a default value to be specified.
        public virtual T? DoAt<T>(Pointer? offset, Func<T> action)
        {
            if (offset == null) 
                return default;

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Pointer? current = HasCurrentPointer ? CurrentPointer : null;
            Goto(offset);

            try
            {
                return action();
            }
            finally
            {
                Goto(current);
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serializes a value
        /// </summary>
        /// <typeparam name="T">The type of value to serialize</typeparam>
        /// <param name="obj">The value to be serialized</param>
        /// <param name="name">A name can be provided optionally, for logging or text serialization purposes</param>
        /// <returns>The value that was serialized</returns>
        public abstract T Serialize<T>(T obj, string? name = null)
            where T : struct;

        public abstract T? SerializeNullable<T>(T? obj, string? name = null)
            where T : struct;

        public abstract bool SerializeBoolean<T>(bool obj, string? name = null)
            where T : struct;

        /// <summary>
        /// Serializes a <see cref="BinarySerializable"/> object
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        /// <param name="name">A name can be provided optionally, for logging or text serialization purposes</param>
        /// <returns>The object that was serialized</returns>
        public abstract T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null) 
            where T : BinarySerializable, new();

        public abstract Pointer? SerializePointer(
            Pointer? obj, 
            PointerSize size = PointerSize.Pointer32, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            long? nullValue = null, 
            string? name = null);

        public abstract Pointer<T> SerializePointer<T>(
            Pointer<T>? obj, 
            PointerSize size = PointerSize.Pointer32, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            long? nullValue = null, 
            string? name = null);

        public abstract string SerializeString(string? obj, long? length = null, Encoding? encoding = null, string? name = null);
        
        public abstract string SerializeLengthPrefixedString<T>(string? obj, Encoding? encoding = null, string? name = null)
            where T : struct;

        /// <summary>
        /// Serializes into an data type which is not serializable. This is useful for data types
        /// declared in libraries or to use with structs.
        /// </summary>
        /// <typeparam name="T">The type to serialize into</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="serializeFunc">The serialize func</param>
        /// <param name="name">The object name</param>
        /// <returns>The serialized object</returns>
        public abstract T SerializeInto<T>(T? obj, SerializeInto<T> serializeFunc, string? name = null)
            where T : new();

        #endregion

        #region Array Helpers

        /// <summary>
        /// Initializes an array with a specific size. This is similar to <see cref="SerializeArraySize{T,U}"/>, except
        /// the size is now determines by the <see cref="length"/> parameter instead.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="obj">The array</param>
        /// <param name="length">The array length</param>
        /// <returns>The initialized array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T?[] InitializeArray<T>(T?[]? obj, long length)
        {
            return obj ?? new T?[length];
        }

        /// <summary>
        /// Performs a serialization action on every item in the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="itemAction"></param>
        /// <param name="name"></param>
        public void DoArray<T>(T?[] obj, DoArrayAction<T> itemAction, string? name = null)
        {
            if (obj == null) 
                throw new ArgumentNullException(nameof(obj));
            if (itemAction == null) 
                throw new ArgumentNullException(nameof(itemAction));

            for (int i = 0; i < obj.Length; i++)
                obj[i] = itemAction(obj[i], i, name: UsesSerializeNames ? $"{name}[{i}]" : null);
        }

        #endregion

        #region Array Serialization

        public abstract T?[] SerializeArraySize<T, U>(T?[]? obj, string? name = null)
            where U : struct;
        public abstract T[] SerializeArray<T>(T[]? obj, long count, string? name = null)
            where T : struct;

        public abstract T?[] SerializeNullableArray<T>(T?[]? obj, long count, string? name = null)
            where T : struct;

        public T[] SerializeObjectArray<T>(T?[]? obj, long count, Action<T>? onPreSerialize, string? name = null)
            where T : BinarySerializable, new()
        {
            return SerializeObjectArray<T>(obj, count, onPreSerialize == null ? (Action<T, int>?)null : (x, _) => onPreSerialize(x), name);
        }
        public abstract T[] SerializeObjectArray<T>(T?[]? obj, long count, Action<T, int>? onPreSerialize = null, string? name = null) 
            where T : BinarySerializable, new();

        public abstract Pointer?[] SerializePointerArray(
            Pointer?[]? obj,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null);
        public abstract Pointer<T>[] SerializePointerArray<T>(
            Pointer<T>?[]? obj,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null);

        public abstract string[] SerializeStringArray(
            string?[]? obj,
            long count,
            long? length = null,
            Encoding? encoding = null,
            string? name = null);

        public abstract string[] SerializeLengthPrefixedStringArray<T>(
            string?[]? obj, 
            long count,
            Encoding? encoding = null, 
            string? name = null) 
            where T : struct;

        public abstract T[] SerializeIntoArray<T>(T?[]? obj, long count, SerializeInto<T> serializeFunc, string? name = null)
            where T : new();

        /// <summary>
        /// Serializes an array of undefined size until a specified condition is met
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="obj">The array</param>
        /// <param name="conditionCheckFunc">The condition for ending the array serialization</param>
        /// <param name="getLastObjFunc">If specified the last value when read will be ignored and this will be used to prepend a value when writing</param>
        /// <param name="name">The name</param>
        /// <returns>The array</returns>
        public abstract T[] SerializeArrayUntil<T>(
            T[]? obj, 
            Func<T, bool> conditionCheckFunc, 
            Func<T>? getLastObjFunc = null, 
            string? name = null)
            where T : struct;
        
        public abstract T?[] SerializeNullableArrayUntil<T>(
            T?[]? obj, 
            Func<T?, bool> conditionCheckFunc, 
            Func<T?>? getLastObjFunc = null, 
            string? name = null)
            where T : struct;

        /// <summary>
        /// Serializes an object array of undefined size until a specified condition is met
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="obj">The object array</param>
        /// <param name="conditionCheckFunc">The condition for ending the array serialization</param>
        /// <param name="getLastObjFunc">If specified the last object when read will be ignored and this will be used to prepend an object when writing</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        /// <param name="name">The name</param>
        /// <returns>The object array</returns>
        public abstract T[] SerializeObjectArrayUntil<T>(
            T?[]? obj, 
            Func<T, bool> conditionCheckFunc, 
            Func<T>? getLastObjFunc = null, 
            Action<T, int>? onPreSerialize = null, 
            string? name = null) 
            where T : BinarySerializable, new();

        public abstract Pointer?[] SerializePointerArrayUntil(
            Pointer?[]? obj,
            Func<Pointer?, bool> conditionCheckFunc,
            Func<Pointer?>? getLastObjFunc = null,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null);

        #endregion

        #region Other Serialization

        public virtual T SerializeFile<T>(string relativePath, T? obj, Action<T>? onPreSerialize = null, string? name = null) 
            where T : BinarySerializable, new()
        {
            Pointer? current = HasCurrentPointer ? CurrentPointer : null;
            Goto(Context.FilePointer(relativePath));

            try
            {
                return SerializeObject<T>(obj, onPreSerialize: onPreSerialize, name: name);
            }
            finally
            {
                Goto(current);
            }
        }

        public T SerializeFromBytes<T>(
            byte[] bytes, 
            string key, 
            Action<T>? onPreSerialize = null, 
            bool removeFile = true, 
            string? name = null)
            where T : BinarySerializable, new()
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return DoAtBytes(bytes, key, () => SerializeObject<T>(default, onPreSerialize, name: name ?? key), removeFile);
        }
        public T DoAtBytes<T>(byte[] bytes, string key, Func<T> func, bool removeFile = true)
        {
            if (bytes == null) 
                throw new ArgumentNullException(nameof(bytes));
            
            try
            {
                if (!Context.FileExists(key))
                {
                    var typeStream = new MemoryStream(bytes);
                    Context.AddFile(new StreamFile(Context, key, typeStream, parentPointer: HasCurrentPointer ? CurrentPointer : null));
                }

                Pointer? current = HasCurrentPointer ? CurrentPointer : null;
                Goto(Context.GetRequiredFile(key).StartPointer);

                try
                {
                    return func();
                }
                finally
                {
                    Goto(current);
                }
            }
            finally
            {
                if (removeFile)
                    Context.RemoveFile(key);
            }
        }

        public void DoAtEncoded(Pointer? offset, IStreamEncoder? encoder, Action action)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            DoAt(offset, () => DoEncoded(encoder, action));
        }

        public void DoWithDefaults(SerializerDefaults? defaults, Action action) 
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            SerializerDefaults? curDefaults = Defaults;
            Defaults = defaults;
            
            try 
            {
                action();
            } 
            finally 
            {
                Defaults = curDefaults;
            }
        }

        public abstract void DoEndian(Endian endianness, Action action);

        public abstract void SerializeBitValues(Action<SerializeBits64> serializeFunc);
        public abstract void DoBits<T>(Action<BitSerializerObject> serializeFunc)
            where T : struct;

        public delegate long SerializeBits64(long value, int length, string? name = null);

        public void SerializePadding(long length, bool logIfNotNull = false, string? name = "Padding")
        {
            if (length == 1)
            {
                byte a = Serialize<byte>(default, name: name);

                if (logIfNotNull && Defaults?.DisableFormattingWarnings != true && a != 0)
                    SystemLogger?.LogWarning("Padding at {0} contains data! Data: 0x{1:X2}", CurrentPointer - length, a);
            }
            else
            {
                byte[] a = SerializeArray<byte>(new byte[length], length, name: name);

                if (logIfNotNull && Defaults?.DisableFormattingWarnings != true && a.Any(x => x != 0))
                    SystemLogger?.LogWarning("Padding at {0} contains data! Data: {1}", CurrentPointer - length, a.ToHexString(align: 16, maxLines: 1));
            }
        }

        public virtual void SerializeMagic<T>(T magic, bool throwIfNoMatch = true, string? name = null)
            where T : struct
        {
            T value = Serialize<T>(magic, name: name ?? "Magic");

            if (!value.Equals(magic))
            {
                if (throwIfNoMatch)
                    throw new Exception($"Magic '{value}' does not match expected magic of '{magic}'");
                else
                    SystemLogger?.LogWarning("Magic '{0}' does not match expected magic of '{1}'", value, magic);
            }
        }

        public virtual void SerializeMagicString(
            string? magic, 
            long length, 
            Encoding? encoding = null, 
            bool throwIfNoMatch = true, 
            string? name = null)
        {
            var value = SerializeString(magic, length, encoding: encoding, name: name ?? "Magic");

            if (value != magic)
            {
                if (throwIfNoMatch)
                    throw new Exception($"Magic '{value}' does not match expected magic of '{magic}'");
                else
                    SystemLogger?.LogWarning("Magic '{0}' does not match expected magic of '{1}'", value, magic);
            }
        }

        #endregion

        #region Caching

        public virtual Task FillCacheForReadAsync(long length) => Task.CompletedTask;

        #endregion

        #region Settings

        public T GetRequiredSettings<T>() where T : class => Context.GetRequiredSettings<T>();

        #endregion

        #region Pre-Defined Pointers

        public virtual Pointer GetRequiredPreDefinedPointer(string key) =>
            Context.GetRequiredPreDefinedPointer(key, CurrentBinaryFile);
        public virtual Pointer? GetPreDefinedPointer(string key) =>
            Context.GetPreDefinedPointer(key, CurrentBinaryFile);

        public virtual Pointer GetRequiredPreDefinedPointer(Enum key) => 
            Context.GetRequiredPreDefinedPointer(key, CurrentBinaryFile);
        public virtual Pointer? GetPreDefinedPointer(Enum key) =>
            Context.GetPreDefinedPointer(key, CurrentBinaryFile);

        #endregion
    }
}