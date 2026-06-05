#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerializer
{
    public class Context : IDisposable
    {
        #region Constructors

        public Context(
            string basePath, 
            ISerializerSettings? settings = null, 
            ISerializerLogger? serializerLogger = null, 
            IFileManager? fileManager = null, 
            ISystemLogger? systemLogger = null)
        {
            // Set properties from parameters
            FileManager = fileManager ?? new DefaultFileManager();
            SystemLogger = systemLogger;
            BasePath = NormalizePath(basePath, true);
            Settings = settings ?? new SerializerSettings();
            SerializerLogger = serializerLogger ?? new EmptySerializerLogger();

            // Initialize properties
            MemoryMap = new MemoryMap();
            Cache = new SerializableCache(SystemLogger);
            ObjectStorage = new Dictionary<string, object>();
            AdditionalSettings = new Dictionary<Type, object>();
        }

        #endregion

        #region Abstraction

        public IFileManager FileManager { get; }
        public ISystemLogger? SystemLogger { get; }

        #endregion

        #region Internal Fields

        internal object _threadLock = new();

        #endregion

        #region Public Properties

        public string BasePath { get; }
        public MemoryMap MemoryMap { get; }
        public SerializableCache Cache { get; }
        public ISerializerLogger SerializerLogger { get; }

        #endregion

        #region Settings

        public ISerializerSettings Settings { get; }

        protected Dictionary<Type, object> AdditionalSettings { get; }
        public T GetRequiredSettings<T>()
            where T : class
        {
            if (!AdditionalSettings.TryGetValue(typeof(T), out object settings))
                throw new ContextException($"The requested serializer settings of type {typeof(T)} could not be found");
            
            return (T)settings;
        }
        public T? GetSettings<T>()
            where T : class
        {
            if (!AdditionalSettings.TryGetValue(typeof(T), out object settings))
                return default;

            return (T)settings;
        }
        public T AddSettings<T>(T settings)
            where T : class
        {
            AdditionalSettings[typeof(T)] = settings ?? throw new ArgumentNullException(nameof(settings));
            return settings;
        }
        public void AddSettings(object settings, Type settingsType)
        {
            if (settingsType == null) 
                throw new ArgumentNullException(nameof(settingsType));
            
            AdditionalSettings[settingsType] = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        public void RemoveSettings<T>()
            where T : class
        {
            AdditionalSettings.Remove(typeof(T));
        }
        public bool HasSettings<T>()
            where T : class
        {
            return AdditionalSettings.ContainsKey(typeof(T));
        }

        public Encoding DefaultEncoding => Settings.DefaultStringEncoding;
        public bool CreateBackupOnWrite => Settings.CreateBackupOnWrite;
        public bool SavePointersForRelocation => Settings.SavePointersForRelocation;

        #endregion

        #region Storage

        protected Dictionary<string, object> ObjectStorage { get; }

        public T GetRequiredStoredObject<T>(string id)
            where T : class
        {
            if (id == null) 
                throw new ArgumentNullException(nameof(id));
            
            if (!ObjectStorage.TryGetValue(id, out object obj))
                throw new ContextException($"The requested object with ID {id} could not be found");

            return (T)obj;
        }

        public T GetRequiredStoredObject<T>()
            where T : class
        {
            return GetRequiredStoredObject<T>(typeof(T).FullName);
        }

        public T? GetStoredObject<T>(string id)
            where T : class
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            
            if (!ObjectStorage.TryGetValue(id, out object obj))
                return default;

            return (T)obj;
        }

        public T? GetStoredObject<T>()
            where T : class
        {
            return GetStoredObject<T>(typeof(T).FullName);
        }

        public void RemoveStoredObject(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            ObjectStorage.Remove(id);
        }

        public void RemoveStoredObject<T>()
        {
            ObjectStorage.Remove(typeof(T).FullName);
        }

        public T StoreObject<T>(string id, T obj)
            where T : class
        {
            if (id == null) 
                throw new ArgumentNullException(nameof(id));
            
            ObjectStorage[id] = obj ?? throw new ArgumentNullException(nameof(obj));
            return obj;
        }

        public T StoreObject<T>(T obj)
            where T : class
        {
            return StoreObject(typeof(T).FullName, obj);
        }

        #endregion

        #region Pre-Defined Pointers

        protected Dictionary<string, long>? PreDefinedPointers { get; set; }

        public void AddPreDefinedPointer(string key, long pointer)
        {
            if (key == null) 
                throw new ArgumentNullException(nameof(key));
            
            PreDefinedPointers ??= new Dictionary<string, long>();

            PreDefinedPointers[key] = pointer;
        }
        public void AddPreDefinedPointer(Enum key, long pointer)
        {
            if (key == null) 
                throw new ArgumentNullException(nameof(key));
            
            PreDefinedPointers ??= new Dictionary<string, long>();

            PreDefinedPointers[key.ToString()] = pointer;
        }
        public void AddPreDefinedPointers(IReadOnlyCollection<KeyValuePair<string, long>> pointers)
        {
            if (pointers == null) 
                throw new ArgumentNullException(nameof(pointers));
            
            PreDefinedPointers ??= new Dictionary<string, long>();

            foreach (var p in pointers)
                PreDefinedPointers[p.Key] = p.Value;
        }
        public void AddPreDefinedPointers<T>(IReadOnlyCollection<KeyValuePair<T, long>> pointers)
            where T : Enum
        {
            if (pointers == null) 
                throw new ArgumentNullException(nameof(pointers));
            
            PreDefinedPointers ??= new Dictionary<string, long>();

            foreach (var p in pointers)
                PreDefinedPointers[p.Key.ToString()] = p.Value;
        }

        public Pointer GetRequiredPreDefinedPointer(string key, BinaryFile file)
        {
            if (key == null) 
                throw new ArgumentNullException(nameof(key));
            if (file == null) 
                throw new ArgumentNullException(nameof(file));
            
            if (PreDefinedPointers == null || !PreDefinedPointers.TryGetValue(key, out long value))
                throw new ContextException($"Pre-defined pointer with key {key} was not found in the context using file {file.FilePath}");

            return new Pointer(value, file);
        }
        public Pointer? GetPreDefinedPointer(string key, BinaryFile file)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (PreDefinedPointers == null || !PreDefinedPointers.TryGetValue(key, out long value))
                return null;

            return new Pointer(value, file);
        }
        public Pointer GetRequiredPreDefinedPointer(Enum key, BinaryFile file)
        {
            if (key == null) 
                throw new ArgumentNullException(nameof(key));
            if (file == null) 
                throw new ArgumentNullException(nameof(file));

            return GetRequiredPreDefinedPointer(key.ToString(), file);
        }
        public Pointer? GetPreDefinedPointer(Enum key, BinaryFile file)
        {
            if (key == null) 
                throw new ArgumentNullException(nameof(key));
            if (file == null) 
                throw new ArgumentNullException(nameof(file));

            return GetPreDefinedPointer(key.ToString(), file);
        }

        #endregion

        #region Files

        private char SeparatorChar => FileManager.SeparatorCharacter switch
        {
            PathSeparatorChar.ForwardSlash => '/',
            PathSeparatorChar.BackSlash => '\\',
            _ => '/',
        };

        public Stream GetFileStream(string relativePath)
        {
            if (relativePath == null) 
                throw new ArgumentNullException(nameof(relativePath));
            
            return FileManager.GetFileReadStream(GetAbsoluteFilePath(NormalizePath(relativePath, false)));
        }

        public BinaryFile GetRequiredFile(string relativePath)
        {
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            BinaryFile? file = GetFile(relativePath);

            if (file == null)
                throw new ContextException($"The requested file {relativePath} was not found");

            return file;
        }
        public BinaryFile? GetFile(string relativePath)
        {
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            string path = NormalizePath(relativePath, false);
            return MemoryMap.Files.FirstOrDefault(f => f.FilePath?.ToLower() == path.ToLower() || f.Alias?.ToLower() == relativePath.ToLower());
        }
        public BinaryFile? GetMemoryMappedFileForAddress(long address, Pointer? anchor = null)
        {
            // Get all memory mapped files
            IEnumerable<BinaryFile> files = MemoryMap.Files.Where(x => x.IsMemoryMapped);

            // Sort based on the priority
            files = files.OrderByDescending(file => file.MemoryMappedPriority);

            // Return the first successful file
            return files.
                Select(f => f.GetLocalPointerFile(address, anchor)).
                FirstOrDefault(p => p != null);
        }

        public virtual string GetAbsoluteFilePath(string relativePath)
        {
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            return BasePath + relativePath;
        }

        public virtual string NormalizePath(string path, bool isDirectory)
        {
            if (path == null) 
                throw new ArgumentNullException(nameof(path));
            
            // Get the path separator character
            string separatorChar = SeparatorChar.ToString();

            // Normalize the path
            string newPath = FileManager.SeparatorCharacter switch
            {
                PathSeparatorChar.ForwardSlash => path.Replace('\\', '/'),
                PathSeparatorChar.BackSlash => path.Replace('/', '\\'),
                _ => throw new ArgumentOutOfRangeException(nameof(FileManager.SeparatorCharacter), FileManager.SeparatorCharacter, null)
            };

            // Make sure a directory path ends with the separator
            if (isDirectory && !newPath.EndsWith(separatorChar) && !String.IsNullOrWhiteSpace(path)) 
                newPath += separatorChar;
            
            return newPath;
        }

        public T AddFile<T>(T file)
            where T : BinaryFile
        {
            if (file == null) 
                throw new ArgumentNullException(nameof(file));
            
            if (MemoryMap.Files.Any(x => x.FilePath == file.FilePath))
                throw new ContextException($"A file with the path '{file.FilePath}' has already been added to the context");

            MemoryMap.Files.Add(file);

            SystemLogger?.LogTrace("Added file {0}", file.FilePath);

            return file;
        }
        public void RemoveFile(string? filePath, bool clearCache = false)
        {
            if (filePath == null) 
                return;

            RemoveFile(GetFile(filePath), clearCache);
        }
        public void RemoveFile(BinaryFile? file, bool clearCache = false)
        {
            if (file is null)
                return;

            MemoryMap.Files.Remove(file);
            deserializer?.DisposeFile(file);
            serializer?.DisposeFile(file);
            file.Dispose();

            if (clearCache)
            {
                // Remove saved pointers
                MemoryMap.Pointers.RemoveAll(x => x.File == file);

                // Remove structs
                Cache.ClearForFile(file);
            }

            SystemLogger?.LogTrace("Removed file {0}", file.FilePath);
        }

        public Pointer<T> FilePointer<T>(string relativePath) 
            where T : BinarySerializable, new()
        {
            if (relativePath == null) 
                throw new ArgumentNullException(nameof(relativePath));
            
            return new Pointer<T>(FilePointer(relativePath));
        }
        public Pointer FilePointer(string relativePath)
        {
            if (relativePath == null) 
                throw new ArgumentNullException(nameof(relativePath));
            
            return GetRequiredFile(relativePath).StartPointer;
        }
        public bool FileExists(BinaryFile? file)
        {
            if (file == null)
                return false;

            return MemoryMap.Files.Contains(file);
        }
        public bool FileExists(string? relativePath)
        {
            if (relativePath == null)
                return false;

            return GetFile(relativePath) != null;
        }

        public T? GetMainFileObject<T>(string? relativePath) 
            where T : BinarySerializable
        {
            if (relativePath == null)
                return null;

            return GetMainFileObject<T>(GetFile(relativePath));
        }
        public T? GetMainFileObject<T>(BinaryFile? file) 
            where T : BinarySerializable
        {
            if (file == null)
                return null;

            Pointer ptr = file.StartPointer;
            return Cache.FromOffset<T>(ptr);
        }

        #endregion

        #region Serializers

        private BinaryDeserializer? deserializer;
        public BinaryDeserializer Deserializer
        {
            get
            {
                if (deserializer == null)
                {
                    if (serializer != null)
                    {
                        serializer.Dispose();
                        serializer = null;
                    }
                    deserializer = new BinaryDeserializer(this);
                }
                return deserializer;
            }
        }

        private BinarySerializer? serializer;
        public BinarySerializer Serializer
        {
            get
            {
                if (serializer == null)
                {
                    if (deserializer != null)
                    {
                        deserializer.Dispose();
                        deserializer = null;
                    }
                    serializer = new BinarySerializer(this);
                }
                return serializer;
            }
        }

        #endregion

        #region Events

        public event EventHandler? Disposed;

        #endregion

        #region Dispose

        public void Close()
        {
            deserializer?.Dispose();
            deserializer = null;
            serializer?.Dispose();
            serializer = null;

            foreach (var file in MemoryMap.Files)
                file?.Dispose();

            SerializerLogger.Dispose();
        }
        public void Dispose()
        {
            Close();
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}