using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding ABGR-1555
    /// </summary>
    public class ABGR1555Color : BaseBitwiseColor
    {
        public ABGR1555Color() { }
        public ABGR1555Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public ABGR1555Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Alpha] = new ColorChannelFormat(0, 1),
            [ColorChannel.Blue] = new ColorChannelFormat(1, 5),
            [ColorChannel.Green] = new ColorChannelFormat(6, 5),
            [ColorChannel.Red] = new ColorChannelFormat(11, 5),
        };
    }
}