using System.Runtime.InteropServices;

namespace BinarySerializer
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct UInt24
    {
        public UInt24(uint value)
        {
            _b0 = (byte)(value & 0xFF);
            _b1 = (byte)((value >> 8) & 0xFF);
            _b2 = (byte)((value >> 16) & 0xFF);
        }

        public const uint MaxValue = 0xFFFFFF;
        public const uint MinValue = 0;

        private readonly byte _b0;
        private readonly byte _b1;
        private readonly byte _b2;

        public uint Value => (uint)(_b0 | (_b1 << 8) | (_b2 << 16));

        public static implicit operator uint(UInt24 d) => d.Value;
        public static explicit operator UInt24(uint b) => new UInt24(b);

        public override string ToString() => Value.ToString();
    }
}