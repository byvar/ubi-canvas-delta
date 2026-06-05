using System;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-777
    /// </summary>
    public class RGB777Color : BaseBytewiseRGBColor
    {
        public RGB777Color() { }
        public RGB777Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public override float Red
        {
            get => (R & 0x7F) / 127f;
            set => R = (byte)Math.Round(value * 127);
        }
        public override float Green
        {
            get => (G & 0x7F) / 127f;
            set => G = (byte)Math.Round(value * 127);
        }
        public override float Blue
        {
            get => (B & 0x7F) / 127f;
            set => B = (byte)Math.Round(value * 127);
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