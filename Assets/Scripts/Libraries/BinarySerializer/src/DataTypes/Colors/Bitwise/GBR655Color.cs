using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding GBR-655
    /// </summary>
    public class GBR655Color : BaseBitwiseColor
    {
        public GBR655Color() { }
        public GBR655Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public GBR655Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Green] = new ColorChannelFormat(0, 6),
            [ColorChannel.Blue] = new ColorChannelFormat(6, 5),
            [ColorChannel.Red] = new ColorChannelFormat(11, 5),
        };
    }
}