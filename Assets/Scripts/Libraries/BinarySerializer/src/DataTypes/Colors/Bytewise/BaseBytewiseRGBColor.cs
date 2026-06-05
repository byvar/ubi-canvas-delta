namespace BinarySerializer
{
    public abstract class BaseBytewiseRGBColor : BaseColor
    {
        protected BaseBytewiseRGBColor() { }
        protected BaseBytewiseRGBColor(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}