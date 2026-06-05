#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace BinarySerializer
{
    public abstract class BinaryFile : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The context the file belongs to</param>
        /// <param name="filePath">The file path relative to the main directory in the context</param>
        /// <param name="endianness">The endianness to use when serializing the file</param>
        /// <param name="baseAddress">The base address for the file. If the file is not memory mapped this should be 0.</param>
        /// <param name="startPointer">The start pointer for the file. If null it will be the same as <see cref="BaseAddress"/></param>
        /// <param name="memoryMappedPriority">e file priority if memory mapped. Default is the address if set to -1.</param>
        protected BinaryFile(
            Context context, 
            string filePath, 
            Endian? endianness = null, 
            long baseAddress = 0, 
            Pointer? startPointer = null, 
            long memoryMappedPriority = -1)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            FilePath = Context.NormalizePath(FilePath, false);
            AbsolutePath = Context.GetAbsoluteFilePath(FilePath);
            Endianness = endianness ?? context.Settings.DefaultEndianness;
            BaseAddress = baseAddress;
            StartPointer = startPointer ?? new Pointer(baseAddress, this);
            MemoryMappedPriority = memoryMappedPriority == -1 ? baseAddress : memoryMappedPriority;
            FileRedirectBehavior = RedirectBehavior.None;

            if (context.Settings.AutoInitReadMap)
                InitFileReadMap();
        }

        #endregion

        #region Abstraction

        protected IFileManager FileManager => Context.FileManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The context the file belongs to
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// The endianness to use when serializing the file
        /// </summary>
        public Endian Endianness { get; }

        /// <summary>
        /// Files can be identified with an alias besides <see cref="FilePath"/>
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// The file path relative to the main directory in the context
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The absolute path to the file
        /// </summary>
        public string AbsolutePath { get; }

        /// <summary>
        /// The base address for the file. If the file is not memory mapped this should be 0.
        /// </summary>
        public long BaseAddress { get; }

        /// <summary>
        /// The length of the file
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// The start pointer for the file
        /// </summary>
        public Pointer StartPointer { get; }

        /// <summary>
        /// The file priority if memory mapped. Default is the address.
        /// </summary>
        public long MemoryMappedPriority { get; }

        /// <summary>
        /// Indicates if the file should be treated as being memory mapped
        /// </summary>
        public abstract bool IsMemoryMapped { get; }

        /// <summary>
        /// Indicates if pointers leading to this file should be saved to the Memory Map
        /// </summary>
        public virtual bool SavePointersToMemoryMap => true;

        /// <summary>
        /// Indicates if objects read from this file should not be cached
        /// </summary>
        public bool IgnoreCacheOnRead { get; set; }

        private PointerSize? _pointerSize;
        public virtual PointerSize PointerSize
        {
            get
            {
                if (_pointerSize == null)
                {
                    if (BaseAddress + Length > UInt32.MaxValue)
                        _pointerSize = PointerSize.Pointer64;
                    else
                        _pointerSize = PointerSize.Pointer32;
                }

                return _pointerSize.Value;
            }
        }

        /// <summary>
        /// The specified behavior to use when serializing (writing) a pointer which has an invalid file associated with it
        /// </summary>
        public RedirectBehavior FileRedirectBehavior { get; set; }

        /// <summary>
        /// The file to redirect to if <see cref="FileRedirectBehavior"/> is set to <see cref="RedirectBehavior.SpecifiedFile"/>
        /// </summary>
        public BinaryFile? RedirectFile { get; set; }

        #endregion

        #region Methods

        public abstract Reader CreateReader();
        public abstract Writer CreateWriter();

        /// <summary>
        /// Retrieves the <see cref="BinaryFile"/> for a serialized <see cref="Pointer"/> value
        /// </summary>
        /// <param name="serializedValue">The serialized pointer value</param>
        /// <param name="anchor">An optional anchor for the pointer</param>
        /// <returns></returns>
        public virtual BinaryFile? GetPointerFile(long serializedValue, Pointer? anchor = null)
        {
            if (IsMemoryMapped)
                return GetMemoryMappedPointerFile(serializedValue, anchor);
            else
                return GetLocalPointerFile(serializedValue, anchor);
        }

        public virtual BinaryFile? GetLocalPointerFile(long serializedValue, Pointer? anchor = null)
        {
            long anchorOffset = anchor?.AbsoluteOffset ?? 0;

            if (serializedValue + anchorOffset >= BaseAddress && serializedValue + anchorOffset <= BaseAddress + Length)
                return this;

            return null;
        }

        public virtual bool TryGetPointer(
            long value, 
            out Pointer? result, 
            Pointer? anchor = null, 
            bool allowInvalid = false, 
            PointerSize size = PointerSize.Pointer32) 
        {
            BinaryFile? file = GetPointerFile(value, anchor);
            Pointer? ptr = null;

            if (file != null)
                ptr = new Pointer(value, file, anchor, size);

            result = ptr;

            if (ptr == null && value != 0 && !allowInvalid && !AllowInvalidPointer(value, anchor: anchor))
                return false;

            return true;
        }

        public virtual long GetPointerValueToSerialize(Pointer? obj, Pointer? anchor = null, long? nullValue = null) => 
            obj?.SerializedOffset ?? nullValue ?? 0;

        public virtual BinaryFile? GetMemoryMappedPointerFile(long serializedValue, Pointer? anchor = null)
        {
            return Context.GetMemoryMappedFileForAddress(serializedValue, anchor);
        }

        public virtual bool AllowInvalidPointer(long serializedValue, Pointer? anchor = null) => false;

        public virtual void EndRead(Reader? reader)
        {
            reader?.Dispose();
        }
        public virtual void EndWrite(Writer? writer)
        {
            writer?.Flush();
            writer?.Dispose();
        }

        public virtual void Dispose()
        {
            if (Context.Settings.AutoExportReadMap && FileReadMap != null)
            {
                try
                {
                    ExportFileReadMap(Context.GetAbsoluteFilePath($"{FilePath}.map"));
                }
                catch (Exception ex)
                {
                    Context.SystemLogger?.Log(LogLevel.Error, "Failed to auto export read map: {0}", ex.Message);
                }
            }
        }

        #endregion

        #region Override Pointers

        protected Dictionary<long, Pointer?>? OverridePointers { get; set; }

        public virtual void AddOverridePointer(long offset, Pointer? pointer)
        {
            OverridePointers ??= new Dictionary<long, Pointer?>();
            OverridePointers.Add(offset, pointer);
        }

        public virtual bool TryGetOverridePointer(long offset, out Pointer? value)
        {
            if (OverridePointers == null)
            {
                value = null;
                return false;
            }

            return OverridePointers.TryGetValue(offset, out value);
        }

        #endregion

        #region File Map

        public virtual bool[]? FileReadMap { get; protected set; }
        [MemberNotNullWhen(true, nameof(FileReadMap))]
        public bool ShouldUpdateReadMap => FileReadMap != null;
        protected bool ShouldInitFileReadMap { get; set; }

        public void InitFileReadMap() => ShouldInitFileReadMap = true;
        public void InitFileReadMap(long length, bool forceInit = false)
        {
            if (forceInit || ShouldInitFileReadMap)
            {
                ShouldInitFileReadMap = false;
                FileReadMap = new bool[length];
            }
        }

        public void UpdateReadMap(long offset, long length)
        {
            if (!ShouldUpdateReadMap)
                return;

            for (int i = 0; i < length; i++)
                FileReadMap[offset + i] = true;
        }
        public void ExportFileReadMap(string outputFilePath)
        {
            if (outputFilePath == null) 
                throw new ArgumentNullException(nameof(outputFilePath));
            
            if (FileReadMap == null)
                throw new InvalidOperationException("Can not export a file read map before it has been initialized");

            File.WriteAllBytes(outputFilePath, FileReadMap.Select(x => (byte)(x ? 0xFF : 0x00)).ToArray());
        }

        #endregion

        #region Region

        protected SortedList<long, Region>? Regions { get; set; }

        public void AddRegion(long offset, long length, string name)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));
            
            Regions ??= new SortedList<long, Region>();
            Regions[offset] = new Region(offset, length, name);
        }

        public Region? GetRegion(long offset)
        {
            if (Regions == null) 
                return null;

            // Binary search
            int lower = 0;
            int upper = Regions.Count - 1;
            var keys = Regions.Keys;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                var val = Regions[keys[middle]];

                if (offset < val.Offset)
                    upper = middle - 1;
                else if (offset >= val.Offset && offset < val.Offset + val.Length)
                    return val;
                else
                    lower = middle + 1;
            }

            return null;
        }

        public class Region
        {
            public Region(long offset, long length, string name)
            {
                Offset = offset;
                Length = length;
                Name = name;
            }

            public long Offset { get; }
            public long Length { get; }
            public string Name { get; }
        }

        #endregion

        #region Labels

        protected Dictionary<long, string>? Labels { get; set; }

        public void AddLabel(long offset, string label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            Labels ??= new Dictionary<long, string>();
            Labels[offset] = label;
        }

        public string? GetLabel(long offset) =>
            Labels != null && Labels.TryGetValue(offset, out string value) ? value : null;

        #endregion

        #region Data Types

        public enum RedirectBehavior
        {
            /// <summary>
            /// Doesn't change the file
            /// </summary>
            None,

            /// <summary>
            /// Throws an exception
            /// </summary>
            Throw,

            /// <summary>
            /// Changes the file to the current one
            /// </summary>
            CurrentFile,

            /// <summary>
            /// Changes the file to the one specified by <see cref="RedirectFile"/>
            /// </summary>
            SpecifiedFile,
        }

        #endregion
    }
}