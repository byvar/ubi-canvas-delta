using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-565
    /// </summary>
    public class BGR565Color : BaseBitwiseColor
    {
        public BGR565Color() { }
        public BGR565Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public BGR565Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Blue] = new ColorChannelFormat(0, 5),
            [ColorChannel.Green] = new ColorChannelFormat(5, 6),
            [ColorChannel.Red] = new ColorChannelFormat(11, 5),
        };
    }
}