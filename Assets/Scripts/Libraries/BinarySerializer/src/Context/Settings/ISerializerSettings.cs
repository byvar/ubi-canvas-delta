#nullable enable
using System.Text;

namespace BinarySerializer
{
    /// <summary>
    /// Interface for serializer settings
    /// </summary>
    public interface ISerializerSettings
    {
        /// <summary>
        /// The default string encoding to use when none is specified
        /// </summary>
        Encoding DefaultStringEncoding { get; }

        /// <summary>
        /// The default endianness to use when creating new files
        /// </summary>
        Endian DefaultEndianness { get; }
        
        /// <summary>
        /// Indicates if a backup file should be created when writing to a file
        /// </summary>
        bool CreateBackupOnWrite { get; }

        /// <summary>
        /// Indicates if pointers should be saved in the Memory Map for relocation
        /// </summary>
        bool SavePointersForRelocation { get; }

        /// <summary>
        /// Indicates if caching read objects should be ignored
        /// </summary>
        bool IgnoreCacheOnRead { get; }

        /// <summary>
        /// Indicates if the default should be to check and log if bytes skipped for an alignment are not null
        /// </summary>
        bool LogAlignIfNotNull { get; }

        /// <summary>
        /// The pointer size to use when logging a <see cref="Pointer"/>. Set to <see langword="null"/> to dynamically determine the appropriate size.
        /// </summary>
        PointerSize? LoggingPointerSize { get; }

        /// <summary>
        /// Indicates if files should have their read map automatically be initialized
        /// </summary>
        public bool AutoInitReadMap { get; }

        /// <summary>
        /// Indicates if files should have their read map automatically be exported on dispose
        /// </summary>
        public bool AutoExportReadMap { get; }
    }
}