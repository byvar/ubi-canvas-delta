namespace BinarySerializer
{
    /// <summary>
    /// Array of simple non-R1Serializable types. Mainly for files that are just simple arrays. Use <see cref="SerializerObject.SerializeArray"/> where possible
    /// </summary>
    /// <typeparam name="T">Generic value parameter, should not be <see cref="BinarySerializable"/></typeparam>
    public class Array<T> : BinarySerializable
        where T : struct
    {
        public long Pre_Length { get; set; } = 0;
        public T[] Value { get; set; }

        public static implicit operator T[](Array<T> array) => array.Value;
        public static implicit operator Array<T>(T[] array) => new Array<T>() { Pre_Length = array.Length, Value = array };

        public override void SerializeImpl(SerializerObject s) 
        {
            Value = s.SerializeArray<T>(Value, Pre_Length, name: "Value");
        }
    }
}