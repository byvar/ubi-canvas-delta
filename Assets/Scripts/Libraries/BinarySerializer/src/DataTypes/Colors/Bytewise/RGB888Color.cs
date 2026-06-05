using System;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-888
    /// </summary>
    public class RGB888Color : BaseBytewiseRGBColor
    {
        public RGB888Color() { }
        public RGB888Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public override float Red
        {
            get => R / 255f;
            set => R = (byte)Math.Round(value * 255);
        }
        public override float Green
        {
            get => G / 255f;
            set => G = (byte)Math.Round(value * 255);
        }
        public override float Blue
        {
            get => B / 255f;
            set => B = (byte)Math.Round(value * 255);
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