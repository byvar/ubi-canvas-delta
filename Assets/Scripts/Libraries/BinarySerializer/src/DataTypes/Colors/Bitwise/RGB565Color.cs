using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-565
    /// </summary>
    public class RGB565Color : BaseBitwiseColor
    {
        public RGB565Color() { }
        public RGB565Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public RGB565Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Red] = new ColorChannelFormat(0, 5),
            [ColorChannel.Green] = new ColorChannelFormat(5, 6),
            [ColorChannel.Blue] = new ColorChannelFormat(11, 5),
        };
    }
}