using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGBA-4444
    /// </summary>
    public class RGBA4444Color : BaseBitwiseColor
    {
        public RGBA4444Color() { }
        public RGBA4444Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public RGBA4444Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Red] = new ColorChannelFormat(0, 4),
            [ColorChannel.Green] = new ColorChannelFormat(4, 4),
            [ColorChannel.Blue] = new ColorChannelFormat(8, 4),
            [ColorChannel.Alpha] = new ColorChannelFormat(12, 4),
        };
    }
}