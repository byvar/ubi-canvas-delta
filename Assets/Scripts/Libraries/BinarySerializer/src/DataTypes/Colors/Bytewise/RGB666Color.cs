using System;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-666
    /// </summary>
    public class RGB666Color : BaseBytewiseRGBColor
    {
        public RGB666Color() { }
        public RGB666Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public override float Red
        {
            get => (R & 0x3F) / 63f;
            set => R = (byte)Math.Round(value * 63);
        }
        public override float Green
        {
            get => (G & 0x3F) / 63f;
            set => G = (byte)Math.Round(value * 63);
        }
        public override float Blue
        {
            get => (B & 0x3F) / 63f;
            set => B = (byte)Math.Round(value * 63);
        }
        public override float Alpha
        {
            get => 1f;
            set { }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            R = s.Serialize<byte>(R, name: nameof(R));
            G = s.Serialize<byte>(G, name: nameof(G));
            B = s.Serialize<byte>(B, name: nameof(B));
        }
    }
}