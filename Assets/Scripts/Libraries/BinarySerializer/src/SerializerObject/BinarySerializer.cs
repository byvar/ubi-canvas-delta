#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BinarySerializer
{
    /// <summary>
    /// A binary serializer used for serializing
    /// </summary>
    public class BinarySerializer : SerializerObject, IDisposable
    {
        #region Constructor

        public BinarySerializer(Context context) : base(context)
        {
            Writers = new Dictionary<BinaryFile, Writer>();
            WrittenObjects = new HashSet<BinarySerializable>(new IdentityComparer<BinarySerializable>());
        }

        #endregion

        #region Protected Properties

        protected HashSet<BinarySerializable> WrittenObjects { get; }
        protected Dictionary<BinaryFile, Writer> Writers { get; }
        protected Writer? Writer { get; set; }
        protected BinaryFile? CurrentFile { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current length of the data being serialized
        /// </summary>
        public override long CurrentLength => Writer?.BaseStream.Length ?? throw new SerializerMissingCurrentPointerException();

        [MemberNotNullWhen(true, nameof(CurrentFile), nameof(Writer))]
        public override bool HasCurrentPointer => CurrentFile != null && Writer != null;

        /// <summary>
        /// The current binary file being used by the serializer
        /// </summary>
        public override BinaryFile CurrentBinaryFile => CurrentFile ?? throw new SerializerMissingCurrentPointerException();

        /// <summary>
        /// The current file offset
        /// </summary>
        public override long CurrentFileOffset => Writer?.BaseStream.Position ?? throw new SerializerMissingCurrentPointerException();

        public override bool UsesSerializeNames => IsSerializerLoggerEnabled;

        #endregion

        #region Logging

        protected string? LogPrefix => IsSerializerLoggerEnabled ? $"(W) {CurrentPointer}:{new string(' ', (Depth + 1) * 2)}" : null;
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
         
            VerifyHasCurrentPointer();
            
            if (encoder == null)
            {
                action();
                return;
            }

            using MemoryStream decodedStream = new();
            
            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Create a temporary file for the stream to serialize to
            StreamFile sf = new(
                context: Context,
                name: key,
                stream: decodedStream,
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers,
                parentPointer: CurrentPointer);

            try
            {
                // Add the temporary file
                Context.AddFile(sf);

                // Serialize the data into the stream
                DoAt(sf.StartPointer, action);

                // Encode the stream to the current file
                decodedStream.Position = 0;
                encoder.EncodeStream(decodedStream, Writer.BaseStream);
            }
            finally
            {
                // Remove the temporary file
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

            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Add the stream
            MemoryStream memStream = new();

            StreamFile sf = new(
                context: Context,
                name: key,
                stream: memStream,
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers,
                parentPointer: CurrentPointer);

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

            VerifyHasCurrentPointer();

            EncodedFiles.Remove(encodedFile);

            encodedFile.Stream.Position = 0;
            encodedFile.Encoder.EncodeStream(encodedFile.Stream, Writer.BaseStream);
            encodedFile.Stream.Close();
            Context.RemoveFile(encodedFile.File);
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

            Writer.AddBinaryProcessor(processor);
        }

        public override void EndProcessed(BinaryProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            VerifyHasCurrentPointer();

            Log("{0}: End processing", processor.GetType().Name);

            if ((processor.Flags & BinaryProcessorFlags.Callbacks) != 0)
                processor.EndProcessing(this);

            Writer.RemoveBinaryProcessor(processor);
        }

        public override T? GetProcessor<T>()
            where T : class
        {
            VerifyHasCurrentPointer();
            return Writer.GetBinaryProcessor<T>();
        }

        #endregion

        #region Positioning

        public override void Goto(Pointer? offset)
        {
            if (offset == null)
            {
                Writer = null;
                CurrentFile = null;
            }
            else
            {
                BinaryFile newFile = offset.File;

                if (newFile != CurrentFile || !HasCurrentPointer)
                {
                    if (!Writers.TryGetValue(newFile, out Writer writer))
                    {
                        writer = newFile.CreateWriter();
                        Writers.Add(newFile, writer);
                    }

                    Writer = writer;
                    CurrentFile = newFile;
                }

                Writer.BaseStream.Position = offset.FileOffset;
            }
        }

        public override void Align(int alignBytes = 4, Pointer? baseOffset = null, bool? logIfNotNull = null)
        {
            long align = (CurrentAbsoluteOffset - (baseOffset?.AbsoluteOffset ?? 0)) % alignBytes;

            // Make sure we need to align
            if (align == 0)
                return;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}Align {alignBytes}");

            long count = alignBytes - align;

            // We can ignore logIfNotNull here since we're writing
            Goto(CurrentPointer + count);
        }

        #endregion

        #region Serialization

        public override T Serialize<T>(T obj, string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}) {name ?? DefaultName}: {obj}");

            WriteValue(obj, name);

            return obj;
        }

        public override T? SerializeNullable<T>(T? obj, string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}?) {name ?? DefaultName}: {obj}");

            WriteNullableValue(obj, name);

            return obj;
        }

        public override bool SerializeBoolean<T>(bool obj, string? name = null)
        {
            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}) {name ?? DefaultName}: {obj}");

            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
                Writer.Write((byte)(obj ? 1 : 0));
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
                Writer.Write((short)(obj ? 1 : 0));
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
                Writer.Write((int)(obj ? 1 : 0));
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
                Writer.Write((long)(obj ? 1 : 0));
            else
                throw new UnsupportedDataTypeException($"Can't serialize {typeof(T)} as a boolean");

            return obj;
        }

        public override T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null)
            where T : class
        {
            if (obj == null) 
            {
                obj = new T();
                obj.Init(CurrentPointer);
            }
            else if (WrittenObjects.Contains(obj))
            {
                Goto(CurrentPointer + obj.SerializedSize);
                return obj;
            }
            else
            {
                // Reinitialize object
                obj.Init(CurrentPointer);
            }

            string? logString = LogPrefix;
            bool isLogTemporarilyDisabled = false;

            if (!DisableSerializerLogForObject && obj is ISerializerShortLog)
            {
                DisableSerializerLogForObject = true;
                isLogTemporarilyDisabled = true;
            }

            if (IsSerializerLoggerEnabled) 
                Context.SerializerLogger.Log($"{logString}(Object: {typeof(T)}) {name ?? DefaultName}");

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
                    if (IsSerializerLoggerEnabled)
                    {
                        string shortLog = (obj as ISerializerShortLog)?.ShortLog ?? "null";
                        Context.SerializerLogger.Log($"{logString}({typeof(T)}) {name ?? DefaultName}: {shortLog}");
                    }
                }
            }

            WrittenObjects.Add(obj);

            return obj;
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

            // Redirect pointer files for pointer which have removed files, such as if the pointer is serialized within encoded data
            if (obj != null && !Context.FileExists(obj.File))
            {
                obj = CurrentBinaryFile.FileRedirectBehavior switch
                {
                    BinaryFile.RedirectBehavior.Throw => 
                        throw new ContextException($"The file for the pointer {obj} does not exist in the current context"),
                    
                    BinaryFile.RedirectBehavior.CurrentFile => 
                        new Pointer(obj.AbsoluteOffset, CurrentBinaryFile, obj.Anchor, obj.Size, Pointer.OffsetType.Absolute),
                    
                    BinaryFile.RedirectBehavior.SpecifiedFile when CurrentBinaryFile.RedirectFile != null => 
                        new Pointer(
                            offset: obj.AbsoluteOffset, 
                            file: CurrentBinaryFile.RedirectFile, 
                            anchor: obj.Anchor, 
                            size: obj.Size, 
                            offsetType: Pointer.OffsetType.Absolute),
                    
                    _ => obj
                };
            }

            BinaryFile currentFile = CurrentFile;

            if (Defaults != null)
            {
                if (anchor == null) 
                    anchor = Defaults.PointerAnchor;

                if (Defaults.PointerFile != null)
                    currentFile = Defaults.PointerFile;

                nullValue ??= Defaults.PointerNullValue;
            }

            if (anchor != null && obj != null)
                obj = new Pointer(obj.SerializedOffset, obj.File, anchor, obj.Size);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({size}) {name ?? DefaultName}: {obj}");

            long valueToSerialize = currentFile.GetPointerValueToSerialize(obj, anchor: anchor, nullValue: nullValue);

            switch (size)
            {
                case PointerSize.Pointer16:
                    Writer.Write((ushort)valueToSerialize);
                    break;

                case PointerSize.Pointer32:
                    Writer.Write((uint)valueToSerialize);
                    break;

                case PointerSize.Pointer64:
                    Writer.Write((long)valueToSerialize);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }

            return obj;
        }

        public override Pointer<T> SerializePointer<T>(
            Pointer<T>? obj,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null,
            string? name = null)
        {
            obj ??= new Pointer<T>();

            Pointer? pointerValue = obj.PointerValue;
            pointerValue = SerializePointer(
                obj: pointerValue, 
                size: size, 
                anchor: anchor, 
                allowInvalid: allowInvalid, 
                nullValue: nullValue, 
                name: IsSerializerLoggerEnabled ? $"<{typeof(T)}> {name ?? DefaultName}" : name);
            
            if (obj.PointerValue != pointerValue)
                obj = new Pointer<T>(pointerValue, obj.Value);

            return obj;
        }

        public override string SerializeString(string? obj, long? length = null, Encoding? encoding = null, string? name = null)
        {
            VerifyHasCurrentPointer();

            obj ??= String.Empty;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(String) {name ?? DefaultName}: {obj}");

            encoding ??= Defaults?.StringEncoding ?? Context.DefaultEncoding;

            if (length.HasValue)
                Writer.WriteString(obj, length.Value, encoding);
            else
                Writer.WriteNullDelimitedString(obj, encoding);

            return obj;
        }

        public override string SerializeLengthPrefixedString<T>(string? obj, Encoding? encoding = null, string? name = null)
        {
            VerifyHasCurrentPointer();

            obj ??= String.Empty;

            encoding ??= Defaults?.StringEncoding ?? Context.DefaultEncoding;
            int length = encoding.GetByteCount(obj);
            string lengthName = $"{name ?? DefaultName}.Length";

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(String) {lengthName}: {length}");

            WriteInteger<T>(length, lengthName);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(String) {name ?? DefaultName}: {obj}");

            Writer.WriteString(obj, length, encoding);

            return obj;
        }

        public override T SerializeInto<T>(T? obj, SerializeInto<T> serializeFunc, string? name = null) 
            where T : default
        {
            obj ??= new T();

            string? logString = LogPrefix;
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
            obj ??= Array.Empty<T?>();

            name = $"{name ?? DefaultName}.Length";

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(U).Name}) {name}: {obj}");

            WriteInteger<U>(obj.Length, name);

            return obj;
        }

        public override T[] SerializeArray<T>(T[]? obj, long count, string? name = null)
        {
            VerifyHasCurrentPointer();

            T[] buffer = obj ?? new T[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
            {
                if (typeof(T) == typeof(byte))
                {
                    string normalLog = $"{LogPrefix}({typeof(T).Name}[{count}]) {name ?? DefaultName}: ";
                    string hexStr = ((byte[])(object)buffer).ToHexString(
                        align: 16, 
                        newLinePrefix: new string(' ', normalLog.Length), 
                        maxLines: 10);
                    Context.SerializerLogger.Log(normalLog + hexStr);
                }
                else
                {
                    Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}[{count}]) {name ?? DefaultName}");
                }
            }

            // Use byte writing method if requested
            if (typeof(T) == typeof(byte))
            {
                Writer.Write((byte[])(object)buffer);
                return buffer;
            }

            for (int i = 0; i < count; i++)
                buffer[i] = Serialize<T>(buffer[i], name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer;
        }

        public override T?[] SerializeNullableArray<T>(T?[]? obj, long count, string? name = null)
        {
            VerifyHasCurrentPointer();

            T?[] buffer = obj ?? new T?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}[{count}]) {name ?? DefaultName}");

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeNullable<T>(buffer[i], name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer;
        }

        public override T[] SerializeObjectArray<T>(T?[]? obj, long count, Action<T, int>? onPreSerialize = null, string? name = null)
            where T : class
        {
            T?[] buffer = obj ?? new T?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(Object[] {typeof(T)}[{count}]) {name ?? DefaultName}");

            for (int i = 0; i < count; i++)
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
            Pointer?[] buffer = obj ?? new Pointer?[count];

            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({size}[{count}]) {name ?? DefaultName}");

            for (int i = 0; i < count; i++)
                buffer[i] = SerializePointer(
                    obj: buffer[i],
                    size: size,
                    anchor: anchor,
                    allowInvalid: allowInvalid,
                    nullValue: nullValue,
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

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
            Pointer<T>?[] buffer = obj ?? new Pointer<T>?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(Pointer<{typeof(T)}>[{count}]) {name ?? DefaultName}");

            for (int i = 0; i < count; i++)
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
            string?[] buffer = obj ?? new string?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log(LogPrefix + "(String[" + count + "]) " + (name ?? DefaultName));

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeString(
                    obj: buffer[i], 
                    length, encoding, 
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override string[] SerializeLengthPrefixedStringArray<T>(string?[]? obj, long count, Encoding? encoding = null,
            string? name = null)
        {
            string?[] buffer = obj ?? new string?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log(LogPrefix + "(String[" + count + "]) " + (name ?? DefaultName));

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeLengthPrefixedString<T>(
                    obj: buffer[i],
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[{i}]" : name);

            return buffer!;
        }

        public override T[] SerializeIntoArray<T>(T?[]? obj, long count, SerializeInto<T> serializeFunc, string? name = null) 
            where T : default
        {
            T?[] buffer = obj ?? new T?[count];
            count = buffer.Length;

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}(Into object[] {typeof(T)}[{count}]) {name ?? DefaultName}");

            for (int i = 0; i < count; i++)
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
            obj ??= Array.Empty<T>();

            // Serialize the array
            obj = SerializeArray<T>(obj, obj.Length, name: name);

            // Serialize the terminator value if there is one
            if (getLastObjFunc != null)
                Serialize<T>(getLastObjFunc(), name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[x]" : name);

            return obj;
        }

        public override T?[] SerializeNullableArrayUntil<T>(
            T?[]? obj, 
            Func<T?, bool> conditionCheckFunc, 
            Func<T?>? getLastObjFunc = null, 
            string? name = null)
        {
            obj ??= Array.Empty<T?>();

            // Serialize the array
            obj = SerializeNullableArray<T>(obj, obj.Length, name: name);

            // Serialize the terminator value if there is one
            if (getLastObjFunc != null)
                SerializeNullable<T>(getLastObjFunc(), name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[x]" : name);

            return obj;
        }

        public override T[] SerializeObjectArrayUntil<T>(
            T?[]? obj,
            Func<T, bool> conditionCheckFunc,
            Func<T>? getLastObjFunc = null,
            Action<T, int>? onPreSerialize = null,
            string? name = null)
            where T : class
        {
            obj ??= Array.Empty<T?>();

            // Serialize the array
            obj = SerializeObjectArray<T>(obj, obj.Length, onPreSerialize: onPreSerialize, name: name);

            // Serialize the terminator object if there is one
            if (getLastObjFunc != null)
                SerializeObject<T>(
                    obj: getLastObjFunc(), 
                    onPreSerialize: onPreSerialize != null ? x => onPreSerialize(x, obj.Length) : null, 
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[x]" : name);

            return obj!;
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
            obj ??= Array.Empty<Pointer?>();

            // Serialize the array
            obj = SerializePointerArray(
                obj: obj, 
                count: obj.Length, 
                size: size, 
                anchor: anchor, 
                allowInvalid: allowInvalid, 
                nullValue: nullValue, 
                name: name);

            // Serialize the terminator pointer if there is one
            if (getLastObjFunc != null)
                SerializePointer(
                    obj: getLastObjFunc(),
                    size: size,
                    anchor: anchor,
                    allowInvalid: allowInvalid,
                    nullValue: nullValue,
                    name: IsSerializerLoggerEnabled ? $"{name ?? DefaultName}[x]" : name);

            return obj;
        }

        #endregion

        #region Other Serialization

        public override void DoEndian(Endian endianness, Action action)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            VerifyHasCurrentPointer();

            Writer w = Writer;
            bool isLittleEndian = w.IsLittleEndian;
            
            if (isLittleEndian != (endianness == Endian.Little))
            {
                w.IsLittleEndian = endianness == Endian.Little;
                action();
                w.IsLittleEndian = isLittleEndian;
            }
            else
            {
                action();
            }
        }

        public override void SerializeBitValues(Action<SerializeBits64> serializeFunc) => throw new NotImplementedException();

        public override void DoBits<T>(Action<BitSerializerObject> serializeFunc) 
        {
            if (serializeFunc == null)
                throw new ArgumentNullException(nameof(serializeFunc));

            BitSerializer serializer = new(this, CurrentPointer, LogPrefix, 0);

            // Set bits
            serializeFunc(serializer);

            if (IsSerializerLoggerEnabled)
                Context.SerializerLogger.Log($"{LogPrefix}({typeof(T).Name}) Value: {serializer.Value}");

            WriteInteger<T>(serializer.Value, "Value");
        }

        #endregion

        #region Public Helpers

        public void ClearWrittenObjects() => WrittenObjects.Clear();

        #endregion

        #region Protected Helpers

        [MemberNotNull(nameof(CurrentFile), nameof(Writer))]
        protected void VerifyHasCurrentPointer()
        {
            if (!HasCurrentPointer)
                throw new SerializerMissingCurrentPointerException();
        }

        /// <summary>
        /// Writes a supported value to the stream
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="name">The value name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteValue<T>(T value, string? name)
            where T : struct
        {
            VerifyHasCurrentPointer();

            if (value is bool bo)
            {
                Writer.Write((byte)(bo ? 1 : 0));
            }
            else if (value is sbyte sb)
            {
                Writer.Write((byte)sb);
            }
            else if (value is byte by)
            {
                Writer.Write(by);
            }
            else if (value is short sh)
            {
                Writer.Write(sh);
            }
            else if (value is ushort ush)
            {
                Writer.Write(ush);
            }
            else if (value is int i32)
            {
                Writer.Write(i32);
            }
            else if (value is uint ui32)
            {
                Writer.Write(ui32);
            }
            else if (value is long lo)
            {
                Writer.Write(lo);
            }
            else if (value is ulong ulo)
            {
                Writer.Write(ulo);
            }
            else if (value is float fl)
            {
                Writer.Write(fl);
            }
            else if (value is double dou)
            {
                Writer.Write(dou);
            }
            else if (value is UInt24 u24)
            {
                Writer.Write(u24);
            }
            else if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));

                if (type == typeof(sbyte))
                    Writer.Write(CastTo<sbyte>.From(value));
                else if (type == typeof(byte))
                    Writer.Write(CastTo<byte>.From(value));
                else if (type == typeof(short))
                    Writer.Write(CastTo<short>.From(value));
                else if (type == typeof(ushort))
                    Writer.Write(CastTo<ushort>.From(value));
                else if (type == typeof(int))
                    Writer.Write(CastTo<int>.From(value));
                else if (type == typeof(uint))
                    Writer.Write(CastTo<uint>.From(value));
                else if (type == typeof(long))
                    Writer.Write(CastTo<long>.From(value));
                else if (type == typeof(ulong))
                    Writer.Write(CastTo<ulong>.From(value));
                else
                    throw new NotSupportedException($"The specified enum type {typeof(T)} for {name} can not be written");
            }
            else
            {
                throw new NotSupportedException($"The specified value type {typeof(T)} for {name} can not be written");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteInteger<T>(long value, string? name)
        {
            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(sbyte))
            {
                Writer.Write((sbyte)value);
            }
            else if (typeof(T) == typeof(byte))
            {
                Writer.Write((byte)value);
            }
            else if (typeof(T) == typeof(short))
            {
                Writer.Write((short)value);
            }
            else if (typeof(T) == typeof(ushort))
            {
                Writer.Write((ushort)value);
            }
            else if (typeof(T) == typeof(int))
            {
                Writer.Write((int)value);
            }
            else if (typeof(T) == typeof(uint))
            {
                Writer.Write((uint)value);
            }
            else if (typeof(T) == typeof(long))
            {
                Writer.Write((long)value);
            }
            else if (typeof(T) == typeof(ulong))
            {
                Writer.Write((ulong)value);
            }
            else if (typeof(T) == typeof(float))
            {
                Writer.Write((float)value);
            }
            else if (typeof(T) == typeof(double))
            {
                Writer.Write((double)value);
            }
            else if (typeof(T) == typeof(UInt24))
            {
                Writer.Write((UInt24)value);
            }
            else if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));

                if (type == typeof(sbyte))
                    Writer.Write(CastTo<sbyte>.From(value));
                else if (type == typeof(byte))
                    Writer.Write(CastTo<byte>.From(value));
                else if (type == typeof(short))
                    Writer.Write(CastTo<short>.From(value));
                else if (type == typeof(ushort))
                    Writer.Write(CastTo<ushort>.From(value));
                else if (type == typeof(int))
                    Writer.Write(CastTo<int>.From(value));
                else if (type == typeof(uint))
                    Writer.Write(CastTo<uint>.From(value));
                else if (type == typeof(long))
                    Writer.Write(CastTo<long>.From(value));
                else if (type == typeof(ulong))
                    Writer.Write(CastTo<ulong>.From(value));
                else
                    throw new NotSupportedException($"The specified enum type {typeof(T)} for {name} can not be written");
            }
            else
            {
                throw new NotSupportedException($"The specified value type {typeof(T)} for {name} can not be written");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteNullableValue<T>(T? value, string? name)
            where T : struct
        {
            VerifyHasCurrentPointer();

            if (typeof(T) == typeof(sbyte))
            {
                if (value == null)
                    Writer.Write((sbyte)-1);
                else
                    Writer.Write(CastTo<sbyte>.From(value));
            }
            else if (typeof(T) == typeof(byte))
            {
                if (value == null)
                    Writer.Write(Byte.MaxValue);
                else
                    Writer.Write(CastTo<byte>.From(value));
            }
            else if (typeof(T) == typeof(short))
            {
                if (value == null)
                    Writer.Write((short)-1);
                else
                    Writer.Write(CastTo<short>.From(value));
            }
            else if (typeof(T) == typeof(ushort))
            {
                if (value == null)
                    Writer.Write(UInt16.MaxValue);
                else
                    Writer.Write(CastTo<ushort>.From(value));
            }
            else if (typeof(T) == typeof(int))
            {
                if (value == null)
                    Writer.Write((int)-1);
                else
                    Writer.Write(CastTo<int>.From(value));
            }
            else if (typeof(T) == typeof(uint))
            {
                if (value == null)
                    Writer.Write(UInt32.MaxValue);
                else
                    Writer.Write(CastTo<uint>.From(value));
            }
            else if (typeof(T) == typeof(long))
            {
                if (value == null)
                    Writer.Write((long)-1);
                else
                    Writer.Write(CastTo<long>.From(value));
            }
            else if (typeof(T) == typeof(ulong))
            {
                if (value == null)
                    Writer.Write(UInt64.MaxValue);
                else
                    Writer.Write(CastTo<ulong>.From(value));
            }
            else if (typeof(T) == typeof(UInt24))
            {
                if (value == null)
                    Writer.Write(UInt24.MaxValue);
                else
                    Writer.Write(CastTo<UInt24>.From(value));
            }
            else
            {
                throw new NotSupportedException($"The specified nullable value type {typeof(T)} for {name} can not be read from the reader");
            }
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            foreach (KeyValuePair<BinaryFile, Writer> w in Writers)
                w.Key.EndWrite(w.Value);

            Writers.Clear();
            Writer = null;
        }

        public void DisposeFile(BinaryFile? file)
        {
            if (file == null || !Writers.ContainsKey(file)) 
                return;

            Writer w = Writers[file];
            file.EndWrite(w);
            Writers.Remove(file);

            if (CurrentFile == file)
                CurrentFile = null;

            SystemLogger?.LogTrace("Disposed file from serializer {0}", file.FilePath);
        }

        #endregion

        #region Data Types

        private sealed class IdentityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            public bool Equals(T x, T y) => ReferenceEquals(x, y);
            public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }

        #endregion
    }
}