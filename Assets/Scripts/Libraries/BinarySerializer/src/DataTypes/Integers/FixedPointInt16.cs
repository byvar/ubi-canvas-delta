namespace BinarySerializer
{
    public class FixedPointInt16 : BinarySerializable, ISerializerShortLog
    {
        // Set in onPreSerialize
        public int Pre_PointPosition { get; set; } = 8; // By default, the point will be at 8 bits

        public short Value { get; set; }

        public float AsFloat 
        {
            get 
            {
                int divisor = 1 << Pre_PointPosition;
                float val = Value / (float)divisor;
                return val;
            }
            set 
            {
                int divisor = 1 << Pre_PointPosition;
                Value = (short)(value * divisor);
            }
        }
        public static implicit operator float(FixedPointInt16 d) => d?.AsFloat ?? 0f;

        public static implicit operator FixedPointInt16(float f) => new FixedPointInt16() 
        {
            AsFloat = f
        };

        public override void SerializeImpl(SerializerObject s) 
        {
            Value = s.Serialize<short>(Value, name: nameof(Value));
        }

        public string ShortLog => ToString();
        public override string ToString() => AsFloat.ToString();
    }
}