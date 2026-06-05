namespace BinarySerializer
{
    public abstract class BaseBytewiseRGBAColor : BaseBytewiseRGBColor
    {
        protected BaseBytewiseRGBAColor() { }
        protected BaseBytewiseRGBAColor(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public byte A { get; set; }
    }
}