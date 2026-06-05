#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace BinarySerializer
{
    public class Pointer<T>
    {
        public Pointer(Pointer? pointerValue, T? value = default)
        {
            Context = pointerValue?.Context;
            PointerValue = pointerValue;
            Value = value;
        }
        public Pointer()
        {
            PointerValue = null;
            Value = default;
        }

        public Context? Context { get; private set; }
        public Pointer? PointerValue { get; }
        public T? Value { get; set; }

        [MemberNotNull(nameof(Context))]
        public Pointer<T> ResolveValue(SerializerObject s, SerializeFunction<T> func, IStreamEncoder? encoder = null) 
        {
            if (s == null) 
                throw new ArgumentNullException(nameof(s));
            if (func == null) 
                throw new ArgumentNullException(nameof(func));

            Context = s.Context;

            if (PointerValue == null) 
                return this;

            if (encoder == null)
                s.DoAt(PointerValue, () => Value = func(s, Value, name: nameof(Value)));
            else
                s.DoAt(PointerValue, () => s.DoEncoded(encoder, () => Value = func(s, Value, name: nameof(Value))));

            return this;
        }

        public static implicit operator T?(Pointer<T> a) => a.Value;
        public static implicit operator Pointer?(Pointer<T>? a) => a?.PointerValue;

        public static Pointer<U> FromObject<U>(U? obj) 
            where U : BinarySerializable, new() 
        {
            if (obj == null) 
                return new Pointer<U>();

            return new Pointer<U>(obj.Offset, value: obj);
        }
    }
}