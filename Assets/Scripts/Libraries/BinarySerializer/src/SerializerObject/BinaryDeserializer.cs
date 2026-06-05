#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{
    /// <summary>
    /// A binary serializer used for deserializing
    /// </summary>
    public class BinaryDeserializer : SerializerObject, IDisposable
    {
        #region Constructor

        public BinaryDeserializer(Context context) : base(context)
        {
            Readers = new Dictionary<BinaryFile, Reader>();
        }

        #endregion

        #region Protected Properties

        protected Dictionary<BinaryFile, Reader> Readers { get; }
        protected Reader? Reader { get; set; }
        protected BinaryFile? CurrentFile { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current length of the data being serialized
        /// </summary>
        public override long CurrentLength => Reader?.BaseStream.Length ?? throw new SerializerMissingCurrentPointerException();

        [MemberNotNullWhen(true, nameof(CurrentFile), nameof(Reader))]
        public override bool HasCurrentPointer => CurrentFile != null && Reader != null;

        /// <summary>
        /// The current binary file being used by the serializer
        /// </summary>
        public override BinaryFile CurrentBinaryFile => CurrentFile ?? throw new SerializerMissingCurrentPointerException();

        /// <summary>
        /// The current file offset
        /// </summary>
        public override long CurrentFileOffset => Reader?.BaseStream.Position ?? throw new SerializerMissingCurrentPointerException();

        public override bool UsesSerializeNames => IsSerializerLoggerEnabled;

        #endregion

        #region Logging

        protected string? LogPrefix => IsSerializerLoggerEnabled ? $"(R) {CurrentPointer}:{new string(' ', (Depth + 1) * 2)}" : null;
        public override void Log(string logString, params object?[] args)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log(LogPrefix + String.Format(logString, args));
        }

        #endregion

        #region Encoding
        
        public override void DoEncoded(
            IStreamEncoder? encoder, 
            Action action, 
            Endian? endianness = null, 
            bool allowLocalPointers = false, 
            string? filename = null)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            if (encoder == null)
            {
                action();
                return;
            }

            VerifyHasCurrentPointer();

            Pointer offset = CurrentPointer;
            long start = Reader.BaseStream.Position;

            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Decode the data into a stream
            using MemoryStream memStream = new();
            encoder.DecodeStream(Reader.BaseStream, memStream);
            memStream.Position = 0;
            long encodedLength = CurrentFileOffset - offset.FileOffset;

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            // Add the stream
            StreamFile sf = new(
                context: Context,
                name: key,
                stream: memStream,
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers,
                parentPointer: offset);

            // Uncomment to test if encoding a stream works using the specified encoder
            /*
            {
                try
                {
                    using var compMemStream = new MemoryStream();
                    encoder.EncodeStream(memStream, compMemStream);
                    memStream.Position = 0;
                    compMemStream.Position = 0;
                    using var decompMemStream = new MemoryStream();
                    encoder.DecodeStream(compMemStream, decompMemStream);

                    if (!decompMemStream.GetBuffer().SequenceEqual(memStream.GetBuffer()))
                        throw new Exception($"Stream encoding failed at {offset}");
                    else
                        Context.Logger.Log("Stream encoding succeeded at {0}", offset);
                }
                catch (NotImplementedException)
                {
                    memStream.Position = 0;
                }
            }
            */

            try
            {
                Context.AddFile(sf);

                DoAt(sf.StartPointer, () =>
                {
                    Log("Decoded data using {0} at {1} with decoded length {2} and encoded length {3}",
                        encoder.Name, offset, sf.Length, encodedLength);

                    action();

                    if (CurrentPointer != sf.StartPointer + sf.Length)
                        SystemLogger?.LogWarning("Encoded block {0} was not fully deserialized: Serialized size: {1} != Total size: {2}", 
                            key, CurrentPointer - sf.StartPointer, sf.Length);
                });
            }
            finally
            {
                Context.RemoveFile(sf);
            }
        }

        public override Pointer BeginEncoded(
            IStreamEncoder encoder,
            Endian? endianness = null, 
            bool allowLocalPointers = false, 
            string? filename = null)
        {
            if (encoder == null) 
                throw new ArgumentNullException(nameof(encoder));

            VerifyHasCurrentPointer();
            
            Pointer offset = CurrentPointer;
            long start = Reader.BaseStream.Position;

            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Add the stream
            var memStream = new MemoryStream();
            encoder.DecodeStream(Reader.BaseStream, memStream);
            memStream.Position = 0;

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            StreamFile sf = new(
                context: Context, 
                name: key, 
                stream: memStream, 
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers,
                parentPointer: offset);
            Context.AddFile(sf);
            EncodedFiles.Add(new EncodedState(memStream, sf, encoder));

            return sf.StartPointer;
        }

        public override void EndEncoded(Pointer endPointer)
        {
            if (endPointer == null) 
                throw new ArgumentNullException(nameof(endPointer));
            
            EncodedState? encodedFile = EncodedFiles.FirstOrDefault(ef => ef.File == endPointer.File);

            if (encodedFile == null) 
                return;
            
            EncodedFiles.Remove(encodedFile);

            StreamFile sf = encodedFile.File;
            string key = sf.FilePath;
            
            if (endPointer != sf.StartPointer + sf.Length)
                SystemLogger?.LogWarning("Encoded block {0} was not fully deserialized: Serialized size: {1} != Total size: {2}", 
                    key, endPointer - sf.StartPointer, sf.Length);

            Context.RemoveFile(sf);
            encodedFile.Stream.Close();
        }

        #endregion

        #region Processing

        public override void BeginProcessed(BinaryProcessor processor)
        {
            if (processor == null) 
                throw new ArgumentNullException(nameof(processor));

            VerifyHasCurrentPointer();

            Log("{0}: Begin processing", processor.GetType().Name);

            if ((processor.Flags & BinaryProcessorFlags.Callbacks) != 0)
                processor.BeginProcessing(this);

            Reader.AddBinaryProcessor(processor);
        }

        public override void EndProcessed(BinaryProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            VerifyHasCurrentPointer();

            Log("{0}: End processing", processor.GetType().Name);

            if ((processor.Flags & BinaryProcessorFlags.Callbacks) != 0)
                processor.EndProcessing(this);
            
            Reader.RemoveBinaryProcessor(processor);
        }

        public override T? GetProcessor<T>() 
            where T : class
        {
            VerifyHasCurrentPointer();
            return Reader.GetBinaryProcessor<T>();
        }

        #endregion

        #region Positioning

        public override void Goto(Pointer? offset)
        {
            if (offset == null)
            {
                Reader = null;
                CurrentFile = null;
            }
            else
            {
                BinaryFile newFile = offset.File;

                if (newFile != CurrentFile || !HasCurrentPointer)
                {
                    if (!Readers.TryGetValue(newFile, out Reader reader))
                    {
                        reader = newFile.CreateReader();
                        Readers.Add(newFile, reader);
                        newFile.InitFileReadMap(Readers[newFile].BaseStream.Length);
                    }

                    Reader = reader;
                    CurrentFile = newFile;
                }

                Reader.BaseStream.Position = offset.FileOffset;
            }
        }

        public override void Align(int alignBytes = 4, Pointer? baseOffset = null, bool? logIfNotNull = null)
        {
            VerifyHasCurrentPointer();

            long align = (CurrentAbsoluteOffset - (baseOffset?.AbsoluteOffset ?? 0)) % alignBytes;

            // Make sure we need to align
            if (align == 0) 
                return;
            
            long count = alignBytes - align;

            string? logPrefix = LogPrefix;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logPrefix}Align {alignBytes}");

            logIfNotNull ??= Context.Settings.LogAlignIfNotNull;

            // If we log we have to read the bytes to check that they are not null
            if (logIfNotNull.Value)
            {
                byte[] bytes = Reader.ReadBytes((int)count);

                // Check if any bytes are not null
                if (bytes.Any(x => x != 0))
                {
                    if (IsSerializerLoggerEnabled)
                    {
                        string log = $"{logPrefix}({nameof(Byte)}[{count}]) Align: ";
                        log += bytes.ToHexString(align: 16, newLinePrefix: new string(' ', log.Length), maxLines: 10);
                        Context.SerializerLogger.Log(log);
                    }

                    SystemLogger?.LogWarning("Align bytes at {0} contains data! Data: {1}", 
                        CurrentPointer - count, bytes.ToHexString(align: 16, maxLines: 1));
                }
            }
            else
            {
                Goto(CurrentPointer + count);
            }
        }

        #endregion

        #region Serialization

        public override T Serialize<T>(T obj, string? name = null)
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            long start = Reader.BaseStream.Position;

            T t = ReadValue<T>(name);

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}) {name ?? DefaultName}: {t}");

            return t;
        }

        public override T? SerializeNullable<T>(T? obj, string? name = null)
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            long start = Reader.BaseStream.Position;

            T? t = ReadNullableValue<T>(name);

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}?) {name ?? DefaultName}: {t?.ToString() ?? "null"}");

            return t;
        }

        public override bool SerializeBoolean<T>(bool obj, string? name = null)
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            long start = Reader.BaseStream.Position;

            long value = ReadInteger<T>(name);

            if (value != 0 && value != 1 && Defaults?.DisableFormattingWarnings != true)
            {
                SystemLogger?.LogWarning("Binary boolean '{0}' ({1}) at {2} was not correctly formatted", 
                    name, value, CurrentPointer - Marshal.SizeOf<T>());

                if (IsSerializerLoggerEnabled)
                    Context.SerializerLogger.Log($"{LogPrefix} ({typeof(T)}): Binary boolean was not correctly formatted ({value})");
            }

            obj = value != 0;

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}) {name ?? DefaultName}: {obj}");

            return obj;
        }

        public override T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null)
            where T : class
        {
            VerifyHasCurrentPointer();

            bool ignoreCacheOnRead = CurrentFile.IgnoreCacheOnRead || Context.Settings.IgnoreCacheOnRead;

            // Get the current pointer
            Pointer current = CurrentPointer;

            // Attempt to get a cached instance of the object if caching is enabled
            T? instance = ignoreCacheOnRead ? null : Context.Cache.FromOffset<T>(current);

            // If we did not get a cached object we create a new one
            if (instance == null)
            {
                // Create a new instance
                instance = new T();

                // Initialize the instance
                instance.Init(current);

                // Cache the object if caching is enabled
                if (!ignoreCacheOnRead)
                    Context.Cache.Add<T>(instance);

                string? logString = IsSerializerLoggerEnabled ? LogPrefix : null;
                bool isLogTemporarilyDisabled = false;
                if (!DisableSerializerLogForObject && instance is ISerializerShortLog)
                {
                    DisableSerializerLogForObject = true;
                    isLogTemporarilyDisabled = true;
                }

                if (IsSerializerLoggerEnabled) 
                    Context.SerializerLogger.Log($"{logString}(Object: {typeof(T)}) {name ?? DefaultName}");

                try
                {
                    Depth++;
                    onPreSerialize?.Invoke(instance);
                    instance.Serialize(this);
                }
                finally
                {
                    Depth--;

                    if (isLogTemporarilyDisabled)
                    {
                        DisableSerializerLogForObject = false;
                        if (IsSerializerLoggerEnabled)
                        {
                            string shortLog = (instance as ISerializerShortLog)?.ShortLog ?? "null";
                            Context.SerializerLogger.Log($"{logString}({typeof(T)}) {name ?? DefaultName}: {shortLog}");
                        }
                    }
                }
            }
            else
            {
                Goto(current + instance.SerializedSize);
            }
            return instance;
        }

        public override Pointer? SerializePointer(
            Pointer? obj, 
            PointerSize size = PointerSize.Pointer32, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            long? nullValue = null, 
            string? name = null)
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            BinaryFile currentFile = CurrentFile;

            if (Defaults != null) 
            {
                if (anchor == null)
                    anchor = Defaults.PointerAnchor;

                if (Defaults.PointerFile != null)
                    currentFile = Defaults.PointerFile;

                nullValue ??= Defaults.PointerNullValue;
            }

            // Attempt to get an override pointer from the current file
            bool isOverride = CurrentFile.TryGetOverridePointer(CurrentAbsoluteOffset, out Pointer? ptr);

            long start = Reader.BaseStream.Position;

            // Read the pointer value
            long value = size switch
            {
                PointerSize.Pointer16 => Reader.ReadUInt16(),
                PointerSize.Pointer32 => Reader.ReadUInt32(),
                PointerSize.Pointer64 => Reader.ReadInt64(),
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };

            // Process the value if it's not null
            if (!nullValue.HasValue || value != nullValue)
            {
                // Process the value if there was no override
                if (!isOverride)
                {
                    if (!currentFile.TryGetPointer(value, out ptr, anchor: anchor, allowInvalid: allowInvalid, size: size)) 
                    {
                        // Invalid pointer
                        if (IsSerializerLoggerEnabled)
                            Context.SerializerLogger.Log($"{logString}({size}) {name ?? DefaultName}: InvalidPointerException - {value:X16}");

                        throw new PointerException($"Not a valid pointer at {CurrentPointer - ((int)size)}: {value:X16}", nameof(SerializePointer));
                    }
                }
                // If we have an override, check if there's an anchor and use that
                else if (ptr != null && anchor != null)
                {
                    ptr = new Pointer(ptr.AbsoluteOffset, ptr.File, anchor, ptr.Size, Pointer.OffsetType.Absolute);
                }
            }

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({size}) {name ?? DefaultName}: {ptr}");

            return ptr;
        }

        public override Pointer<T> SerializePointer<T>(
            Pointer<T>? obj, 
            PointerSize size = PointerSize.Pointer32, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            long? nullValue = null, 
            string? name = null)
        {
            Pointer? pointerValue = SerializePointer(
                obj: null, 
                size: size, 
                anchor: anchor, 
                allowInvalid: allowInvalid, 
                nullValue: nullValue, 
                name: IsSerializerLoggerEnabled ? $"<{typeof(T)}> {name ?? DefaultName}" : name);
            return new Pointer<T>(pointerValue);
        }

        public override string SerializeString(string? obj, long? length = null, Encoding? encoding = null, string? name = null)
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            long origPos = Reader.BaseStream.Position;

            encoding ??= Defaults?.StringEncoding ?? Context.DefaultEncoding;

            string t = length.HasValue ? Reader.ReadString(length.Value, encoding) : Reader.ReadNullDelimitedString(encoding);

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(origPos, Reader.BaseStream.Position - origPos);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}(String) {name ?? DefaultName}: {t}");

            return t;
        }

        public override string SerializeLengthPrefixedString<T>(string? obj, Encoding? encoding = null, string? name = null)
        {
            VerifyHasCurrentPointer();

            string? lengthLogString = LogPrefix;
            string lengthName = $"{name ?? DefaultName}.Length";

            long start = Reader.BaseStream.Position;

            long length = ReadInteger<T>(lengthName);

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{lengthLogString}({typeof(T).Name}) {lengthName}: {length}");

            return SerializeString(obj, length: length, encoding: encoding, name: name);
        }

        public override T SerializeInto<T>(T? obj, SerializeInto<T> serializeFunc, string? name = null) 
            where T : default
        {
            VerifyHasCurrentPointer();

            // Create a new instance
            obj = new T();

            string? logString = IsSerializerLoggerEnabled ? LogPrefix : null;
            bool isLogTemporarilyDisabled = false;
            if (!DisableSerializerLogForObject && obj is ISerializerShortLog)
            {
                DisableSerializerLogForObject = true;
                isLogTemporarilyDisabled = true;
            }

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}(Into object: {typeof(T)}) {name ?? DefaultName}");

            try
            {
                Depth++;
                obj = serializeFunc(this, obj);
            }
            finally
            {
                Depth--;

                if (isLogTemporarilyDisabled)
                {
                    DisableSerializerLogForObject = false;
                    if (IsSerializerLoggerEnabled)
                    {
                        string shortLog = (obj as ISerializerShortLog)?.ShortLog ?? "null";
                        Context.SerializerLogger.Log($"{logString}({typeof(T)}) {name ?? DefaultName}: {shortLog}");
                    }
                }
            }

            return obj;
        }

        #endregion

        #region Array Serialization

        public override T?[] SerializeArraySize<T, U>(T?[]? obj, string? name = null)
            where T : default
        {
            VerifyHasCurrentPointer();

            string? logString = LogPrefix;
            name = $"{name ?? DefaultName}.Length";

            long start = Reader.BaseStream.Position;

            long size = ReadInteger<U>(name);

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({typeof(U).Name}) {name}: {size}");

            if (obj == null)
                obj = new T[size];
            else if (obj.Length != size)
                Array.Resize(ref obj, (int)size);

            return obj;
        }

        public override T[] SerializeArray<T>(T[]? obj, long count, string? name = null)
        {
            VerifyHasCurrentPointer();

            // Use byte reading method if requested
            if (typeof(T) == typeof(byte))
            {
                if (CurrentFile.ShouldUpdateReadMap)
                    CurrentFile.UpdateReadMap(Reader.BaseStream.Position, count);

                if (IsSerializerLoggerEnabled)
                {
                    string normalLog = $"{LogPrefix}({typeof(T).Name}[{count}]) {name ?? DefaultName}: ";
                    byte[] bytes = Reader.ReadBytes((int)count);
                    Context.SerializerLogger.Log(normalLog + 
                                    bytes.ToHexString(align: 16, newLinePrefix: new string(' ', normalLog.Length), maxLines: 10));
                    return (T[])(object)bytes;
                }
                else
                {
                    return (T[])(object)Reader.ReadBytes((int)count);
                }
            }
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}[{count}]) {name ?? DefaultName}");
            }
            T[] buffer;
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new T[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = Serialize<T>(buffer[i], name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer;
        }

        public override T?[] SerializeNullableArray<T>(T?[]? obj, long count, string? name = null)
        {
            VerifyHasCurrentPointer();

            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}?[{count}]) {name ?? DefaultName}");
            }

            T?[] buffer;
            
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new T?[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeNullable<T>(buffer[i], name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer;
        }

        public override T[] SerializeObjectArray<T>(T?[]? obj, long count, Action<T, int>? onPreSerialize = null, string? name = null)
            where T : class
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}(Object[]: {typeof(T)}[{count}]) {name ?? DefaultName}");
            }
            T?[] buffer;
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new T?[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeObject<T>(
                    obj: buffer[i], 
                    // ReSharper disable once AccessToModifiedClosure
                    onPreSerialize: onPreSerialize == null ? (Action<T>?)null : x => onPreSerialize(x, i), 
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override Pointer?[] SerializePointerArray(
            Pointer?[]? obj,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}({size}[{count}]) {name ?? DefaultName}");
            }

            Pointer?[] buffer;

            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new Pointer?[count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializePointer(buffer[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer;
        }

        public override Pointer<T>[] SerializePointerArray<T>(
            Pointer<T>?[]? obj,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}(Pointer<{typeof(T)}>[{count}]) {name ?? DefaultName}");
            }

            Pointer<T>?[] buffer;

            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new Pointer<T>?[count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializePointer<T>(
                    obj: buffer[i],
                    size: size,
                    anchor: anchor,
                    allowInvalid: allowInvalid,
                    nullValue: nullValue,
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override string[] SerializeStringArray(
            string?[]? obj,
            long count,
            long? length = null,
            Encoding? encoding = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}(String[{count}]) {name ?? DefaultName}");
            }
            string?[] buffer;
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new string?[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeString(buffer[i], length, encoding, name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override string[] SerializeLengthPrefixedStringArray<T>(string?[]? obj, long count, Encoding? encoding = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}(String[{count}]) {name ?? DefaultName}");
            }
            string?[] buffer;
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new string?[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeLengthPrefixedString<T>(buffer[i], encoding, name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override T[] SerializeIntoArray<T>(T?[]? obj, long count, SerializeInto<T> serializeFunc, string? name = null) 
            where T : default
        {
            if (IsSerializerLoggerEnabled)
            {
                string? logString = LogPrefix;
                Context.SerializerLogger.Log($"{logString}(Into object[]: {typeof(T)}[{count}]) {name ?? DefaultName}");
            }

            T?[] buffer;
            if (obj != null)
            {
                buffer = obj;

                if (buffer.Length != count)
                    Array.Resize(ref buffer, (int)count);
            }
            else
            {
                buffer = new T?[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeInto<T>(
                    obj: buffer[i],
                    serializeFunc: serializeFunc,
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override T[] SerializeArrayUntil<T>(
            T[]? obj,
            Func<T, bool> conditionCheckFunc,
            Func<T>? getLastObjFunc = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}[..]) {name ?? DefaultName}");

            List<T> objects = new();
            int index = 0;

            while (true)
            {
                T serializedObj = Serialize<T>(default, name: $"{name ?? DefaultName}[{index}]");

                index++;

                if (conditionCheckFunc(serializedObj))
                {
                    if (getLastObjFunc == null)
                        objects.Add(serializedObj);

                    break;
                }

                objects.Add(serializedObj);
            }

            return objects.ToArray();
        }

        public override T?[] SerializeNullableArrayUntil<T>(
            T?[]? obj, 
            Func<T?, bool> conditionCheckFunc, 
            Func<T?>? getLastObjFunc = null, 
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}?[..]) {name ?? DefaultName}");

            List<T?> objects = new();
            int index = 0;

            while (true)
            {
                T? serializedObj = SerializeNullable<T>(default, name: $"{name ?? DefaultName}[{index}]");

                index++;

                if (conditionCheckFunc(serializedObj))
                {
                    if (getLastObjFunc == null)
                        objects.Add(serializedObj);

                    break;
                }

                objects.Add(serializedObj);
            }

            return objects.ToArray();
        }

        public override T[] SerializeObjectArrayUntil<T>(
            T?[]? obj,
            Func<T, bool> conditionCheckFunc,
            Func<T>? getLastObjFunc = null,
            Action<T, int>? onPreSerialize = null,
            string? name = null)
            where T : class
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(Object[]: {typeof(T)}[..]) {name ?? DefaultName}");

            List<T> objects = new();
            int index = 0;

            while (true)
            {
                T serializedObj = SerializeObject<T>(
                    obj: default, 
                    // ReSharper disable once AccessToModifiedClosure
                    onPreSerialize: onPreSerialize == null ? (Action<T>?)null : x => onPreSerialize(x, index), 
                    name: $"{name ?? DefaultName}[{index}]");

                index++;

                if (conditionCheckFunc(serializedObj))
                {
                    if (getLastObjFunc == null)
                        objects.Add(serializedObj);

                    break;
                }

                objects.Add(serializedObj);
            }

            return objects.ToArray();
        }

        public override Pointer?[] SerializePointerArrayUntil(
            Pointer?[]? obj, 
            Func<Pointer?, bool> conditionCheckFunc, 
            Func<Pointer?>? getLastObjFunc = null,
            PointerSize size = PointerSize.Pointer32, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            long? nullValue = null,
            string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(Pointer[..]) {name ?? DefaultName}");

            List<Pointer?> pointers = new();
            int index = 0;

            while (true)
            {
                Pointer? serializedObj = SerializePointer(
                    obj: default,
                    size: size,
                    anchor: anchor,
                    allowInvalid: allowInvalid,
                    nullValue: nullValue,
                    name: $"{name ?? DefaultName}[{index}]");

                index++;

                if (conditionCheckFunc(serializedObj))
                {
                    if (getLastObjFunc == null)
                        pointers.Add(serializedObj);

                    break;
                }

                pointers.Add(serializedObj);
            }

            return pointers.ToArray();
        }

        #endregion

        #region Other Serialization

        public override void DoEndian(Endian endianness, Action action)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            VerifyHasCurrentPointer();
            
            Reader r = Reader;
            bool isLittleEndian = r.IsLittleEndian;
            
            if (isLittleEndian != (endianness == Endian.Little))
            {
                r.IsLittleEndian = endianness == Endian.Little;
                action();
                r.IsLittleEndian = isLittleEndian;
            }
            else
            {
                action();
            }
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
        {
            if (serializeFunc == null) 
                throw new ArgumentNullException(nameof(serializeFunc));
            
            string? logPrefix = LogPrefix;

            // Extract bits
            int pos = 0;
            
            using MemoryStream buffer = new();
            List<string>? logs = IsSerializerLoggerEnabled ? new List<string>() : null;

            serializeFunc((_, length, name) => 
            {
                int bitsFromPrevByte = 0;

                if (pos % 8 != 0)
                    bitsFromPrevByte = 8 - pos % 8;

                int bitsToRead = length - bitsFromPrevByte;
                int bytesToRead = (int)Math.Ceiling(bitsToRead / 8f);

                for (int i = 0; i < bytesToRead; i++)
                    buffer.WriteByte(Serialize<byte>(default, name: IsSerializerLoggerEnabled ? $"Value[{buffer.Length}]" : null));

                long bitValue = BitHelpers.ExtractBits64(buffer.GetBuffer(), length, pos);

                if (IsSerializerLoggerEnabled)
                    logs?.Add($"{logPrefix}  (UInt{length}) {name ?? DefaultName}: {bitValue}");

                pos += length;
                return bitValue;
            });

            if (IsSerializerLoggerEnabled && logs != null)
                foreach (string l in logs)
                    Context.SerializerLogger.Log(l);
        }

        public override void DoBits<T>(Action<BitSerializerObject> serializeFunc) 
        {
            if (serializeFunc == null) 
                throw new ArgumentNullException(nameof(serializeFunc));

            VerifyHasCurrentPointer();

            string? logString = LogPrefix;

            long start = Reader.BaseStream.Position;
            Pointer p = CurrentPointer;

            long value = ReadInteger<T>("Value");

            if (CurrentFile.ShouldUpdateReadMap)
                CurrentFile.UpdateReadMap(start, Reader.BaseStream.Position - start);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{logString}({typeof(T).Name}) Value: {value}");

            serializeFunc(new BitDeserializer(this, p, logString, value));
        }

        #endregion

        #region Caching

        public override Task FillCacheForReadAsync(long length)
        {
            VerifyHasCurrentPointer();
            return FileManager.FillCacheForReadAsync(length, Reader);
        }

        #endregion

        #region Protected Helpers

        [MemberNotNull(nameof(CurrentFile), nameof(Reader))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void VerifyHasCurrentPointer()
        {
            if (!HasCurrentPointer)
                throw new SerializerMissingCurrentPointerException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T ReadValue<T>(string? name = null)
            where T : struct
        {
            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(bool))
            {
                // By default we read a boolean as a byte
                byte b = Reader.ReadByte();

                if (b != 0 && b != 1 && Defaults?.DisableFormattingWarnings != true)
                {
                    SystemLogger?.LogWarning("Binary boolean '{0}' ({1}) at {2} was not correctly formatted", name, b, CurrentPointer - 1);

                    if (IsSerializerLoggerEnabled)
                        Context.SerializerLogger.Log($"{LogPrefix} ({typeof(T)}): Binary boolean was not correctly formatted ({b})");
                }

                return CastTo<T>.From(b != 0);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return CastTo<T>.From(Reader.ReadSByte());
            }
            else if (typeof(T) == typeof(byte))
            {
                return CastTo<T>.From(Reader.ReadByte());
            }
            else if (typeof(T) == typeof(short))
            {
                return CastTo<T>.From(Reader.ReadInt16());
            }
            else if (typeof(T) == typeof(ushort))
            {
                return CastTo<T>.From(Reader.ReadUInt16());
            }
            else if (typeof(T) == typeof(int))
            {
                return CastTo<T>.From(Reader.ReadInt32());
            }
            else if (typeof(T) == typeof(uint))
            {
                return CastTo<T>.From(Reader.ReadUInt32());
            }
            else if (typeof(T) == typeof(long))
            {
                return CastTo<T>.From(Reader.ReadInt64());
            }
            else if (typeof(T) == typeof(ulong))
            {
                return CastTo<T>.From(Reader.ReadUInt64());
            }
            else if (typeof(T) == typeof(float))
            {
                return CastTo<T>.From(Reader.ReadSingle());
            }
            else if (typeof(T) == typeof(double))
            {
                return CastTo<T>.From(Reader.ReadDouble());
            }
            else if (typeof(T) == typeof(UInt24))
            {
                return CastTo<T>.From(Reader.ReadUInt24());
            }
            else if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));

                if (type == typeof(sbyte))
                    return CastTo<T>.From(Reader.ReadSByte());
                else if (type == typeof(byte))
                    return CastTo<T>.From(Reader.ReadByte());
                else if (type == typeof(short))
                    return CastTo<T>.From(Reader.ReadInt16());
                else if (type == typeof(ushort))
                    return CastTo<T>.From(Reader.ReadUInt16());
                else if (type == typeof(int))
                    return CastTo<T>.From(Reader.ReadInt32());
                else if (type == typeof(uint))
                    return CastTo<T>.From(Reader.ReadUInt32());
                else if (type == typeof(long))
                    return CastTo<T>.From(Reader.ReadInt64());
                else if (type == typeof(ulong))
                    return CastTo<T>.From(Reader.ReadUInt64());
            }

            throw new NotSupportedException($"The specified value type {typeof(T)} for {name} can not be read from the reader");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ReadInteger<T>(string? name = null)
            where T : struct
        {
            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(sbyte))
            {
                return Reader.ReadSByte();
            }
            else if (typeof(T) == typeof(byte))
            {
                return Reader.ReadByte();
            }
            else if (typeof(T) == typeof(short))
            {
                return Reader.ReadInt16();
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Reader.ReadUInt16();
            }
            else if (typeof(T) == typeof(int))
            {
                return Reader.ReadInt32();
            }
            else if (typeof(T) == typeof(uint))
            {
                return Reader.ReadUInt32();
            }
            else if (typeof(T) == typeof(long))
            {
                return Reader.ReadInt64();
            }
            else if (typeof(T) == typeof(ulong))
            {
                return (long)Reader.ReadUInt64();
            }
            else if (typeof(T) == typeof(float))
            {
                return (long)Reader.ReadSingle();
            }
            else if (typeof(T) == typeof(double))
            {
                return (long)Reader.ReadDouble();
            }
            else if (typeof(T) == typeof(UInt24))
            {
                return Reader.ReadUInt24();
            }
            else if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));

                if (type == typeof(sbyte))
                    return Reader.ReadSByte();
                else if (type == typeof(byte))
                    return Reader.ReadByte();
                else if (type == typeof(short))
                    return Reader.ReadInt16();
                else if (type == typeof(ushort))
                    return Reader.ReadUInt16();
                else if (type == typeof(int))
                    return Reader.ReadInt32();
                else if (type == typeof(uint))
                    return Reader.ReadUInt32();
                else if (type == typeof(long))
                    return Reader.ReadInt64();
                else if (type == typeof(ulong))
                    return (long)Reader.ReadUInt64();
            }

            throw new NotSupportedException($"The specified integer type {typeof(T)} for {name} can not be read from the reader");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T? ReadNullableValue<T>(string? name = null)
            where T : struct
        {
            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(sbyte))
            {
                var value = Reader.ReadSByte();
                return value == -1 ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(byte))
            {
                var value = Reader.ReadByte();
                return value == Byte.MaxValue ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(short))
            {
                var value = Reader.ReadInt16();
                return value == -1 ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var value = Reader.ReadUInt16();
                return value == UInt16.MaxValue ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(int))
            {
                var value = Reader.ReadInt32();
                return value == -1 ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(uint))
            {
                var value = Reader.ReadUInt32();
                return value == UInt32.MaxValue ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(long))
            {
                var value = Reader.ReadInt64();
                return value == -1 ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var value = Reader.ReadUInt64();
                return value == UInt64.MaxValue ? null : CastTo<T>.From(value);
            }
            else if (typeof(T) == typeof(UInt24))
            {
                var value = Reader.ReadUInt24();
                return value == UInt24.MaxValue ? null : CastTo<T>.From(value);
            }

            throw new NotSupportedException($"The specified nullable value type {typeof(T)} for {name} can not be read from the reader");
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            foreach (KeyValuePair<BinaryFile, Reader> r in Readers)
                r.Key.EndRead(r.Value);

            Readers.Clear();
            Reader = null;
        }

        public void DisposeFile(BinaryFile? file)
        {
            if (file == null || !Readers.ContainsKey(file))
                return;

            Reader r = Readers[file];
            file.EndRead(r);
            Readers.Remove(file);

            if (CurrentFile == file)
                CurrentFile = null;

            SystemLogger?.LogTrace("Disposed file from deserializer {0}", file.FilePath);
        }

        #endregion
    }
}