namespace BinarySerializer
{
    public class FixedPointInt32 : BinarySerializable, ISerializerShortLog
    {
        // Set in onPreSerialize
        public int Pre_PointPosition { get; set; } = 16; // By default, the point will be at 16 bits

        public int Value { get; set; }

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
                Value = (int)(value * divisor);
            }
        }
        public static implicit operator float(FixedPointInt32 d) => d?.AsFloat ?? 0f;

        public static implicit operator FixedPointInt32(float f) => new FixedPointInt32() 
        {
            AsFloat = f
        };

        public override void SerializeImpl(SerializerObject s) 
        {
            Value = s.Serialize<int>(Value, name: nameof(Value));
        }

        public string ShortLog => ToString();
        public override string ToString() => AsFloat.ToString();
    }
}