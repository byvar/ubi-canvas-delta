using System;

namespace BinarySerializer
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGRA-8888
    /// </summary>
    public class BGRA8888Color : BaseBytewiseRGBAColor
    {
        public BGRA8888Color() { }
        public BGRA8888Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        
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
            get => A / 255f;
            set => A = (byte)Math.Round(value * 255);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            B = s.Serialize<byte>(B, name: nameof(B));
            G = s.Serialize<byte>(G, name: nameof(G));
            R = s.Serialize<byte>(R, name: nameof(R));
            A = s.Serialize<byte>(A, name: nameof(A));
        }
    }
}