using System;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-888
    /// </summary>
    public class BGR888Color : BaseBytewiseRGBColor
    {
        public BGR888Color() { }
        public BGR888Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

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
            B = s.Serialize<byte>(B, name: nameof(B));
            G = s.Serialize<byte>(G, name: nameof(G));
            R = s.Serialize<byte>(R, name: nameof(R));
        }
    }
}