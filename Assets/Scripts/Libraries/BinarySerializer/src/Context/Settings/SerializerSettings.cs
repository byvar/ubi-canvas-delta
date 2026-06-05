#nullable enable
using System.Text;

namespace BinarySerializer
{
    /// <summary>
    /// Default class for serializer settings
    /// </summary>
    public class SerializerSettings : ISerializerSettings
    {
        /// <summary>
        /// The default string encoding to use when none is specified
        /// </summary>
        public Encoding DefaultStringEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// The default endianness to use when creating new files
        /// </summary>
        public Endian DefaultEndianness { get; set; } = Endian.Little;

        /// <summary>
        /// Indicates if a backup file should be created when writing to a file
        /// </summary>
        public bool CreateBackupOnWrite { get; set; }

        /// <summary>
        /// Indicates if pointers should be saved in the Memory Map for relocation
        /// </summary>
        public bool SavePointersForRelocation { get; set; }

        /// <summary>
        /// Indicates if caching read objects should be ignored
        /// </summary>
        public bool IgnoreCacheOnRead { get; set; }

        /// <summary>
        /// Indicates if the default should be to check and log if bytes skipped for an alignment are not null
        /// </summary>
        public bool LogAlignIfNotNull => false;

        /// <summary>
        /// The pointer size to use when logging a <see cref="Pointer"/>. Set to <see langword="null"/> to dynamically determine the appropriate size.
        /// </summary>
        public PointerSize? LoggingPointerSize { get; set; } = PointerSize.Pointer32;

        /// <summary>
        /// Indicates if files should have their read map automatically be initialized
        /// </summary>
        public bool AutoInitReadMap { get; set; }

        /// <summary>
        /// Indicates if files should have their read map automatically be exported on dispose
        /// </summary>
        public bool AutoExportReadMap { get; set; }
    }
}