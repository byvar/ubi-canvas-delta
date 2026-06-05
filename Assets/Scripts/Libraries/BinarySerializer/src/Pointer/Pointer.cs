#nullable enable
using System;

namespace BinarySerializer
{
    public class Pointer : IEquatable<Pointer>, IComparable<Pointer> 
    {
        #region Constructor

        public Pointer(
            long offset, 
            BinaryFile file, 
            Pointer? anchor = null, 
            PointerSize size = PointerSize.Pointer32, 
            OffsetType offsetType = OffsetType.Serialized)
        {
            Anchor = anchor;
            File = file ?? throw new ArgumentNullException(nameof(file));
            Size = size;
            Context = file.Context;

            AbsoluteOffset = offsetType switch
            {
                OffsetType.Serialized when anchor != null => anchor.AbsoluteOffset + offset,
                OffsetType.File => offset + file.BaseAddress,
                _ => offset
            };

            if (offsetType == OffsetType.File)
                FileOffset = offset;
            else
                FileOffset = AbsoluteOffset - file.BaseAddress;

            if (Context.SavePointersForRelocation && file.SavePointersToMemoryMap)
                Context.MemoryMap.AddPointer(this);
        }

        #endregion

        #region Public Properties

        public Context Context { get; }
        public Pointer? Anchor { get; }
        public BinaryFile File { get; }
        public PointerSize Size { get; }

        public long AbsoluteOffset { get; }
        public long FileOffset { get; }

        public long SerializedOffset
        {
            get
            {
                long off = AbsoluteOffset;
                if (Anchor != null)
                    off -= Anchor.AbsoluteOffset;
                return off;
            }
        }

        public string StringFileOffset => GetAddressString(FileOffset);
        public string StringAbsoluteOffset => GetAddressString(AbsoluteOffset);

        protected virtual string GetAddressString(long value)
        {
            return Size switch
            {
                PointerSize.Pointer16 => $"{value:X4}",
                PointerSize.Pointer32 => $"{value:X8}",
                PointerSize.Pointer64 => $"{value:X16}",
                _ => $"{value:X8}"
            };
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            long regionOffset = 0;

            // Attempt to get a region
            BinaryFile.Region? region = File.GetRegion(FileOffset);

            // Get the offset within the region
            if (region != null)
                regionOffset = FileOffset - region.Offset;

            // Attempt to get a label
            string? label = File.GetLabel(FileOffset);

            // Create the initial string
            string str = $"{File.FilePath}|0x{GetAddressString(AbsoluteOffset)}";

            // Add file offset if the file is memory mapped
            if (File.BaseAddress != 0) 
                str += $"[0x{GetAddressString(FileOffset)}]";

            // Add serialized offset if an anchor is specified
            if (Anchor != null)
                str += $"<0x{GetAddressString(SerializedOffset)}>";

            // Append region info
            if (region != null) 
                str += $"({region.Name}:0x{GetAddressString(regionOffset)})";

            // Append label
            if (label != null)
                str += $"({label})";

            return str;
        }

        #endregion

        #region Equality and comparison

        public override bool Equals(object? obj) => obj is Pointer p && this == p;
        public override int GetHashCode() => AbsoluteOffset.GetHashCode() ^ File.GetHashCode();
        public bool Equals(Pointer? other) => this == other;
        public int CompareTo(Pointer? other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            return AbsoluteOffset.CompareTo(other.AbsoluteOffset);
        }

        #endregion

        #region Operators

        public static bool operator ==(Pointer? x, Pointer? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return x.AbsoluteOffset == y.AbsoluteOffset && x.File == y.File;
        }
        public static bool operator !=(Pointer? x, Pointer? y) => !(x == y);

        public static Pointer operator +(Pointer x, long y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));

            return new Pointer(x.AbsoluteOffset + y, x.File, anchor: x.Anchor, size: x.Size, offsetType: OffsetType.Absolute);
        }
        public static Pointer operator -(Pointer x, long y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));
            
            return new Pointer(x.AbsoluteOffset - y, x.File, anchor: x.Anchor, size: x.Size, offsetType: OffsetType.Absolute);
        }

        public static long operator +(Pointer x, Pointer y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));
            if (y == null) 
                throw new ArgumentNullException(nameof(y));
            
            return x.AbsoluteOffset + y.AbsoluteOffset;
        }
        public static long operator -(Pointer x, Pointer y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));
            if (y == null) 
                throw new ArgumentNullException(nameof(y));
            
            return x.AbsoluteOffset - y.AbsoluteOffset;
        }

        #endregion

        #region Data Types

        public enum OffsetType
        {
            Serialized,
            Absolute,
            File,
        }

        #endregion
    }
}