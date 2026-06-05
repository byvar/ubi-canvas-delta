namespace BinarySerializer
{
    /// <summary>
    /// Array of serializable objects. Mainly for files that are just simple object arrays. Use <see cref="SerializerObject.SerializeObjectArray"/> when possible
    /// </summary>
    /// <typeparam name="T">Generic parameter, should be a <see cref="BinarySerializable"/></typeparam>
    public class ObjectArray<T> : BinarySerializable 
        where T : BinarySerializable, new() 
    {
        public long Pre_Length { get; set; } = 0;
        public T[] Value { get; set; }

        public static implicit operator T[](ObjectArray<T> array) => array.Value;
        public static implicit operator ObjectArray<T>(T[] array) => new ObjectArray<T>() { Pre_Length = array.Length, Value = array };

        public override void SerializeImpl(SerializerObject s) 
        {
            Value = s.SerializeObjectArray<T>(Value, Pre_Length, name: "Value");
        }
    }
}