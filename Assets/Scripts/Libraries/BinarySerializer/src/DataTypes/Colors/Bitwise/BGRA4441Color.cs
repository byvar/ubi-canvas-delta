using System.Collections.Generic;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGRA-4441
    /// </summary>
    public class BGRA4441Color : BaseBitwiseColor
    {
        public BGRA4441Color() { }
        public BGRA4441Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public BGRA4441Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>()
        {
            [ColorChannel.Blue] = new ColorChannelFormat(0, 4),
            [ColorChannel.Green] = new ColorChannelFormat(4, 4),
            [ColorChannel.Red] = new ColorChannelFormat(8, 4),
            [ColorChannel.Alpha] = new ColorChannelFormat(15, 1),
        };
    }
}