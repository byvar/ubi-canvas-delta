#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerializer
{
    // TODO: Optimize this class more, similarly to what's been done with other serializers

    /// <summary>
    /// A serializer object used for calculating the size of structs. This does not have a source to write to and instead only saves the position of the serialization. Any <see cref="SerializerObject.DoAt"/> call is ignored to avoid serializing unrelated data.
    /// </summary>
    public class SizeCalculationSerializer : SerializerObject, IDisposable
    {
        #region Constructor

        public SizeCalculationSerializer(Context context) : base(context)
        {
            FilePositions = new Dictionary<BinaryFile, long>();
            BinaryProcessors = new List<BinaryProcessor>();
            EncodingSerializer = new BinarySerializer(context);
        }

        #endregion

        #region Protected Properties

        protected Dictionary<BinaryFile, long> FilePositions { get; }
        protected long? CurrentFilePosition
        {
            get
            {
                VerifyHasCurrentPointer();
                return FilePositions.TryGetValue(CurrentFile, out long v) ? v : null;
            }
            set
            {
                VerifyHasCurrentPointer();
                FilePositions[CurrentFile] = value ?? 0;
            }
        }
        protected BinaryFile? CurrentFile { get; set; }
        protected List<BinaryProcessor> BinaryProcessors { get; }
        protected BinarySerializer EncodingSerializer { get; }
        protected bool IsEncoding { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current length of the data being serialized
        /// </summary>
        public override long CurrentLength => IsEncoding ? EncodingSerializer.CurrentLength : 0;

        [MemberNotNullWhen(true, nameof(CurrentFile))]
        public override bool HasCurrentPointer => IsEncoding ? EncodingSerializer.HasCurrentPointer : CurrentFile != null;

        /// <summary>
        /// The current binary file being used by the serializer
        /// </summary>
        public override BinaryFile CurrentBinaryFile => IsEncoding ? EncodingSerializer.CurrentBinaryFile : CurrentFile ?? throw new SerializerMissingCurrentPointerException();

        /// <summary>
        /// The current file offset
        /// </summary>
        public override long CurrentFileOffset => IsEncoding ? EncodingSerializer.CurrentFileOffset : CurrentFilePosition ?? 0;

        public override bool UsesSerializeNames => false;

        #endregion

        #region Logging

        public override void Log(string logString, params object?[] args) { }

        #endregion

        #region Encoding

        public override void DoEncoded(
            IStreamEncoder? encoder,
            Action action,
            Endian? endianness = null,
            bool allowLocalPointers = false,
            string? filename = null)
        {
            if (IsEncoding)
            {
                EncodingSerializer.DoEncoded(encoder, action, endianness, allowLocalPointers, filename);
                return;
            }

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            VerifyHasCurrentPointer();

            if (encoder == null)
            {
                action();
                return;
            }

            // Encode the data into a stream
            using MemoryStream encodedStream = new();
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

            IsEncoding = true;

            try
            {
                // Add the temporary file
                Context.AddFile(sf);

                // Serialize the data into the stream
                DoAt(sf.StartPointer, action);

                // Encode the stream into a memory stream
                decodedStream.Position = 0;
                encoder.EncodeStream(decodedStream, encodedStream);
            }
            finally
            {
                Context.RemoveFile(sf);
                IsEncoding = false;
            }

            CurrentFilePosition += encodedStream.Length;
        }
        
        public override Pointer BeginEncoded(
            IStreamEncoder encoder,
            Endian? endianness = null,
            bool allowLocalPointers = false,
            string? filename = null)
        {
            if (IsEncoding)
                return EncodingSerializer.BeginEncoded(encoder, endianness, allowLocalPointers, filename);

            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            VerifyHasCurrentPointer();

            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Add the stream
            MemoryStream decodedStream = new();

            StreamFile sf = new(
                context: Context,
                name: key,
                stream: decodedStream,
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers,
                parentPointer: CurrentPointer);

            IsEncoding = true;

            Context.AddFile(sf);

            EncodedFiles.Add(new EncodedState(decodedStream, sf, encoder));

            return sf.StartPointer;
        }

        public override void EndEncoded(Pointer endPointer)
        {
            EncodedState? encodedFile = EncodedFiles.FirstOrDefault(ef => ef.File == endPointer.File);

            if (encodedFile == null)
            {
                if (IsEncoding)
                    EncodingSerializer.EndEncoded(endPointer);

                return;
            }

            VerifyHasCurrentPointer();

            EncodedFiles.Remove(encodedFile);

            using MemoryStream encodedStream = new();
            
            // Encode the stream into a memory stream
            encodedFile.Stream.Position = 0;
            encodedFile.Encoder.EncodeStream(encodedFile.Stream, encodedStream);

            encodedFile.Stream.Close();
            Context.RemoveFile(encodedFile.File);
            IsEncoding = false;

            CurrentFilePosition += encodedStream.Length;
        }

        #endregion

        #region Processing

        public override void BeginProcessed(BinaryProcessor processor)
        {
            if (IsEncoding)
            {
                EncodingSerializer.BeginProcessed(processor);
                return;
            }

            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            if ((processor.Flags & BinaryProcessorFlags.Callbacks) != 0)
                processor.BeginProcessing(this);

            BinaryProcessors.Add(processor);
        }

        public override void EndProcessed(BinaryProcessor processor)
        {
            if (IsEncoding)
            {
                EncodingSerializer.EndProcessed(processor);
                return;
            }

            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            if ((processor.Flags & BinaryProcessorFlags.Callbacks) != 0)
                processor.EndProcessing(this);

            BinaryProcessors.Remove(processor);
        }

        public override T? GetProcessor<T>() 
            where T : class
        {
            if (IsEncoding)
                return EncodingSerializer.GetProcessor<T>();

            return BinaryProcessors.OfType<T>().FirstOrDefault();
        }

        #endregion

        #region Positioning

        public override void Goto(Pointer? offset)
        {
            if (IsEncoding)
            {
                EncodingSerializer.Goto(offset);
                return;
            }

            if (offset == null)
            {
                CurrentFilePosition = null;
                CurrentFile = null;
            }
            else
            {
                BinaryFile newFile = offset.File;

                if (newFile != CurrentFile || !HasCurrentPointer)
                {
                    if (!FilePositions.ContainsKey(newFile))
                        FilePositions.Add(newFile, 0);

                    CurrentFile = newFile;
                }

                CurrentFilePosition = offset.FileOffset;
            }
        }

        public override void Align(int alignBytes = 4, Pointer? baseOffset = null, bool? logIfNotNull = null)
        {
            if (IsEncoding)
            {
                EncodingSerializer.Align(alignBytes, baseOffset, logIfNotNull);
                return;
            }

            long align = (CurrentAbsoluteOffset - (baseOffset?.AbsoluteOffset ?? 0)) % alignBytes;

            // Make sure we need to align
            if (align == 0)
                return;

            long count = alignBytes - align;

            Goto(CurrentPointer + count);
        }

        public override void DoAt(Pointer? offset, Action action)
        {
            if (IsEncoding)
                EncodingSerializer.DoAt(offset, action);
        }

        // TODO: Resolve issues with this when not used
        public override T? DoAt<T>(Pointer? offset, Func<T> action)
            where T : default
        {
            if (IsEncoding)
                return EncodingSerializer.DoAt<T>(offset, action);

            return default;
        }

        #endregion

        #region Serialization

        public override T Serialize<T>(T obj, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.Serialize<T>(obj, name);

            ReadType(typeof(T));
            return obj;
        }

        public override T? SerializeNullable<T>(T? obj, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeNullable<T>(obj, name);

            ReadType(typeof(T));
            return obj;
        }

        public override bool SerializeBoolean<T>(bool obj, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeBoolean<T>(obj, name);

            ReadType(typeof(T));
            return obj;
        }

        public override T SerializeObject<T>(T? obj, Action<T>? onPreSerialize = null, string? name = null)
            where T : class
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeObject<T>(obj, onPreSerialize, name);

            obj ??= new T();

            try
            {
                Depth++;

                if (obj.Offset == null)
                    obj.Init(CurrentPointer);

                onPreSerialize?.Invoke(obj);
                obj.Serialize(this);
            }
            finally
            {
                Depth--;
            }
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
            if (IsEncoding)
                return EncodingSerializer.SerializePointer(obj, size, anchor, allowInvalid, nullValue, name);

            CurrentFilePosition += size switch
            {
                PointerSize.Pointer16 => 2,
                PointerSize.Pointer32 => 4,
                PointerSize.Pointer64 => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };

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
            if (IsEncoding)
                return EncodingSerializer.SerializePointer<T>(obj, size, anchor, allowInvalid, nullValue, name);

            try
            {
                Depth++;

                CurrentFilePosition += size switch
                {
                    PointerSize.Pointer16 => 2,
                    PointerSize.Pointer32 => 4,
                    PointerSize.Pointer64 => 8,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
                };
            }
            finally
            {
                Depth--;
            }
            return obj ?? new Pointer<T>();
        }

        public override string SerializeString(string? obj, long? length = null, Encoding? encoding = null, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeString(obj, length, encoding, name);

            obj ??= String.Empty;

            if (length.HasValue)
                CurrentFilePosition += length;
            else
                CurrentFilePosition += (encoding ?? Defaults?.StringEncoding ?? Context.DefaultEncoding).GetBytes(obj + '\0').Length;

            return obj;
        }

        public override string SerializeLengthPrefixedString<T>(string? obj, Encoding? encoding = null, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeLengthPrefixedString<T>(obj, encoding, name);

            ReadType(typeof(T));
            return SerializeString(obj, encoding: encoding, name: name);
        }

        public override T SerializeInto<T>(T? obj, SerializeInto<T> serializeFunc, string? name = null) where T : default
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeInto<T>(obj, serializeFunc, name);

            obj ??= new T();

            try
            {
                Depth++;

                obj = serializeFunc(this, obj);
            }
            finally
            {
                Depth--;
            }
            return obj;
        }

        #endregion

        #region Array Serialization

        public override T?[] SerializeArraySize<T, U>(T?[]? obj, string? name = null)
            where T : default
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeArraySize<T, U>(obj, name);

            obj ??= Array.Empty<T?>();
            U size = (U)Convert.ChangeType(obj.Length, typeof(U));
            Serialize<U>(size);
            return obj;
        }

        public override T[] SerializeArray<T>(T[]? obj, long count, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeArray<T>(obj, count, name);

            T[] buffer = obj ?? new T[count];

            if (typeof(T) == typeof(byte))
            {
                CurrentFilePosition += buffer.Length;
                return buffer;
            }

            for (int i = 0; i < count; i++)
                buffer[i] = Serialize<T>(buffer[i]);

            return buffer;
        }

        public override T?[] SerializeNullableArray<T>(T?[]? obj, long count, string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeNullableArray<T>(obj, count, name);

            T?[] buffer = obj ?? new T?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeNullable<T>(buffer[i]);

            return buffer;
        }

        public override T[] SerializeObjectArray<T>(T?[]? obj, long count, Action<T, int>? onPreSerialize = null, string? name = null)
            where T : class
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeObjectArray<T>(obj, count, onPreSerialize, name);

            T?[] buffer = obj ?? new T?[count];

            for (int i = 0; i < count; i++)
                // ReSharper disable once AccessToModifiedClosure
                buffer[i] = SerializeObject<T>(buffer[i], onPreSerialize: onPreSerialize == null ? (Action<T>?)null : x => onPreSerialize(x, i));

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
            if (IsEncoding)
                return EncodingSerializer.SerializePointerArray(obj, count, size, anchor, allowInvalid, nullValue, name);

            Pointer?[] buffer = obj ?? new Pointer?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializePointer(buffer[i], anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue);

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
            if (IsEncoding)
                return EncodingSerializer.SerializePointerArray<T>(obj, count, size, anchor, allowInvalid, nullValue, name);

            Pointer<T>?[] buffer = obj ?? new Pointer<T>?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializePointer<T>(
                    obj: buffer[i],
                    anchor: anchor,
                    allowInvalid: allowInvalid,
                    nullValue: nullValue);

            return buffer!;
        }

        public override string[] SerializeStringArray(
            string?[]? obj,
            long count,
            long? length = null,
            Encoding? encoding = null,
            string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeStringArray(obj, count, length, encoding, name);

            string?[] buffer = obj ?? new string?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeString(buffer[i], length, encoding);

            return buffer!;
        }

        public override string[] SerializeLengthPrefixedStringArray<T>(string?[]? obj, long count, Encoding? encoding = null,
            string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeLengthPrefixedStringArray<T>(obj, count, encoding, name);

            string?[] buffer = obj ?? new string?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeLengthPrefixedString<T>(buffer[i], encoding);

            return buffer!;
        }

        public override T[] SerializeIntoArray<T>(T?[]? obj, long count, SerializeInto<T> serializeFunc, string? name = null) where T : default
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeIntoArray<T>(obj, count, serializeFunc, name);

            T?[] buffer = obj ?? new T?[count];

            for (int i = 0; i < count; i++)
                buffer[i] = SerializeInto<T>(buffer[i], serializeFunc);

            return buffer!;
        }

        public override T[] SerializeArrayUntil<T>(
            T[]? obj,
            Func<T, bool> conditionCheckFunc,
            Func<T>? getLastObjFunc = null,
            string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeArrayUntil<T>(obj, conditionCheckFunc, getLastObjFunc, name);

            obj ??= Array.Empty<T>();

            // Serialize the array
            obj = SerializeArray<T>(obj, obj.Length, name: name);

            // Serialize the terminator value if there is one
            if (getLastObjFunc != null)
                Serialize<T>(getLastObjFunc());

            return obj;
        }

        public override T?[] SerializeNullableArrayUntil<T>(
            T?[]? obj, 
            Func<T?, bool> conditionCheckFunc, 
            Func<T?>? getLastObjFunc = null,
            string? name = null)
        {
            if (IsEncoding)
                return EncodingSerializer.SerializeNullableArrayUntil<T>(obj, conditionCheckFunc, getLastObjFunc, name);

            obj ??= Array.Empty<T?>();

            // Serialize the array
            obj = SerializeNullableArray<T>(obj, obj.Length, name: name);

            // Serialize the terminator value if there is one
            if (getLastObjFunc != null)
                SerializeNullable<T>(getLastObjFunc());

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
            if (IsEncoding)
                return EncodingSerializer.SerializeObjectArrayUntil<T>(obj, conditionCheckFunc, getLastObjFunc, onPreSerialize, name);

            obj ??= Array.Empty<T?>();

            // Serialize the array
            obj = SerializeObjectArray<T>(obj, obj.Length, onPreSerialize: onPreSerialize, name: name);

            // Serialize the terminator object if there is one
            if (getLastObjFunc != null)
                SerializeObject<T>(
                    obj: getLastObjFunc(), 
                    onPreSerialize: onPreSerialize != null ? x => onPreSerialize(x, obj.Length) : null);

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
            if (IsEncoding)
                return EncodingSerializer.SerializePointerArrayUntil(obj, conditionCheckFunc, getLastObjFunc, size, anchor, allowInvalid, nullValue, name);

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
                    nullValue: nullValue);

            return obj;
        }

        #endregion

        #region Other Serialization

        public override void DoEndian(Endian endianness, Action action)
        {
            if (IsEncoding)
            {
                EncodingSerializer.DoEndian(endianness, action);
                return;
            }

            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            
            action();
        }

        public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
        {
            if (IsEncoding)
            {
                EncodingSerializer.SerializeBitValues(serializeFunc);
                return;
            }

            if (serializeFunc == null) 
                throw new ArgumentNullException(nameof(serializeFunc));
            
            int totalLength = 0;

            serializeFunc((value, length, _) =>
            {
                totalLength += length;
                return value;
            });

            CurrentFilePosition += (int)Math.Ceiling(totalLength / 8f);
        }

        public override void DoBits<T>(Action<BitSerializerObject> serializeFunc) 
        {
            if (IsEncoding)
            {
                EncodingSerializer.DoBits<T>(serializeFunc);
                return;
            }

            // Serialize value
            Serialize<T>((T)Convert.ChangeType(0, typeof(T)));
        }

        #endregion

        #region Protected Helpers

        [MemberNotNull(nameof(CurrentFile))]
        protected void VerifyHasCurrentPointer()
        {
            if (!HasCurrentPointer)
                throw new SerializerMissingCurrentPointerException();
        }

        protected void ReadType(Type type)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            TypeCode typeCode = Type.GetTypeCode(type);

            CurrentFilePosition += typeCode switch
            {
                TypeCode.Boolean => 1,

                TypeCode.SByte => 1,
                TypeCode.Byte => 1,

                TypeCode.Int16 => 2,
                TypeCode.UInt16 => 2,

                TypeCode.Int32 => 4,
                TypeCode.UInt32 => 4,

                TypeCode.Int64 => 8,
                TypeCode.UInt64 => 8,

                TypeCode.Single => 4,
                TypeCode.Double => 8,

                TypeCode.Object when type == typeof(UInt24) => 3,

                _ => throw new NotSupportedException($"The specified type ('{type.Name}') is not supported")
            };
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            FilePositions.Clear();
            EncodingSerializer.Dispose();
        }

        public void DisposeFile(BinaryFile? file)
        {
            if (file == null)
                return;

            FilePositions.Remove(file);
        }

        #endregion
    }
}