using System.Collections.Generic;

namespace BinarySerializer
{
    public class RGB555Color : BaseBitwiseColor
    {
        public RGB555Color() { }
        public RGB555Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public RGB555Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Red] = new ColorChannelFormat(0, 5),
            [ColorChannel.Green] = new ColorChannelFormat(5, 5),
            [ColorChannel.Blue] = new ColorChannelFormat(10, 5),
        };
    }
}