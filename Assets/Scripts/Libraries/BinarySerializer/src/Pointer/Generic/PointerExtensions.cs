#nullable enable
using System;
using System.Text;

namespace BinarySerializer
{
    public static class PointerExtensions
    {
        #region Main

        public static Pointer<T>[] ResolveValue<T>(
            this Pointer<T>?[] ptrs,
            SerializerObject s,
            SerializeFunction<T> func, 
            IStreamEncoder? encoder = null)
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T>();
                else
                    ptr.ResolveValue(s, func, encoder);
            }

            return ptrs!;
        }
        public static Pointer<T>[] ResolveValue<T>(
            this Pointer<T>?[] ptrs,
            SerializerObject s,
            Func<int, SerializeFunction<T>> func,
            IStreamEncoder? encoder = null)
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T>();
                else
                    ptr.ResolveValue(s, func(i), encoder);
            }

            return ptrs!;
        }

        #endregion

        #region Serialization

        // Value
        public static Pointer<T> Resolve<T>(this Pointer<T> ptr, SerializerObject s, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.Serialize<T>(), encoder);
        }
        public static Pointer<T>[] Resolve<T>(this Pointer<T>?[] ptrs, SerializerObject s, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T>();
                else
                    ptr.Resolve(s, encoder);
            }

            return ptrs!;
        }

        // Nullable value
        public static Pointer<T?> ResolveNullable<T>(this Pointer<T?> ptr, SerializerObject s, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeNullable<T>(), encoder);
        }
        public static Pointer<T?>[] ResolveNullable<T>(this Pointer<T?>?[] ptrs, SerializerObject s, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T?>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T?>();
                else
                    ptr.ResolveNullable(s, encoder);
            }

            return ptrs!;
        }

        // Object
        public static Pointer<T> ResolveObject<T>(this Pointer<T> ptr, SerializerObject s, Action<T>? onPreSerialize = null, IStreamEncoder? encoder = null)
            where T : BinarySerializable, new()
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeObject<T>(onPreSerialize: onPreSerialize), encoder);
        }
        public static Pointer<T>[] ResolveObject<T>(this Pointer<T>?[] ptrs, SerializerObject s, Action<T, int>? onPreSerialize = null, IStreamEncoder? encoder = null)
            where T : BinarySerializable, new()
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T>();
                else
                    // ReSharper disable once AccessToModifiedClosure
                    ptr.ResolveObject(s, onPreSerialize: onPreSerialize != null ? x => onPreSerialize(x, i) : null, encoder);
            }

            return ptrs!;
        }

        // Pointer
        public static Pointer<Pointer> ResolvePointer(
            this Pointer<Pointer> ptr,
            SerializerObject s,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializePointer(
                size: size,
                anchor: anchor,
                allowInvalid: allowInvalid,
                nullValue: nullValue), encoder);
        }
        public static Pointer<Pointer<T>> ResolvePointer<T>(
            this Pointer<Pointer<T>> ptr,
            SerializerObject s,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializePointer<T>(
                size: size,
                anchor: anchor,
                allowInvalid: allowInvalid,
                nullValue: nullValue), encoder);
        }

        // String
        public static Pointer<string> ResolveString(
            this Pointer<string> ptr,
            SerializerObject s,
            long? length = null,
            Encoding? encoding = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeString(length: length, encoding: encoding), encoder);
        }
        public static Pointer<string>[] ResolveString(
            this Pointer<string>?[] ptrs,
            SerializerObject s,
            long? length = null,
            Encoding? encoding = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<string>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<string>();
                else
                    ptr.ResolveString(s, length: length, encoding: encoding, encoder);
            }

            return ptrs!;
        }

        // Into
        public static Pointer<T> ResolveInto<T>(this Pointer<T> ptr, SerializerObject s, SerializeInto<T> serializeFunc, IStreamEncoder? encoder = null)
            where T : new()
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (serializeFunc == null) 
                throw new ArgumentNullException(nameof(serializeFunc));

            return ptr.ResolveValue(s, PointerFunctions.SerializeInto<T>(serializeFunc), encoder);
        }
        public static Pointer<T>[] ResolveInto<T>(this Pointer<T>?[] ptrs, SerializerObject s, SerializeInto<T> serializeFunc, IStreamEncoder? encoder = null)
            where T : new()
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (serializeFunc == null)
                throw new ArgumentNullException(nameof(serializeFunc));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T>();
                else
                    ptr.ResolveInto(s, serializeFunc, encoder);
            }

            return ptrs!;
        }

        #endregion

        #region Array Serialization

        public static Pointer<T[]> ResolveArray<T>(this Pointer<T[]> ptr, SerializerObject s, long count, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeArray<T>(count), encoder);
        }
        public static Pointer<T[]>[] ResolveArray<T>(this Pointer<T[]>?[] ptrs, SerializerObject s, long count, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T[]>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T[]>();
                else
                    ptr.ResolveArray(s, count, encoder);
            }

            return ptrs!;
        }

        public static Pointer<T?[]> ResolveNullableArray<T>(this Pointer<T?[]> ptr, SerializerObject s, long count, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeNullableArray<T>(count), encoder);
        }
        public static Pointer<T?[]>[] ResolveNullableArray<T>(this Pointer<T?[]>?[] ptrs, SerializerObject s, long count, IStreamEncoder? encoder = null)
            where T : struct
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T?[]>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T?[]>();
                else
                    ptr.ResolveNullableArray(s, count, encoder);
            }

            return ptrs!;
        }

        public static Pointer<T[]> ResolveObjectArray<T>(
            this Pointer<T[]> ptr,
            SerializerObject s,
            long count,
            Action<T, int>? onPreSerialize = null, 
            IStreamEncoder? encoder = null)
            where T : BinarySerializable, new()
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeObjectArray<T>(count, onPreSerialize: onPreSerialize), encoder);
        }

        public static Pointer<Pointer?[]> ResolvePointerArray(
            this Pointer<Pointer?[]> ptr,
            SerializerObject s,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializePointerArray(
                count: count,
                size: size,
                anchor: anchor,
                allowInvalid: allowInvalid,
                nullValue: nullValue), encoder);
        }

        public static Pointer<Pointer<T>[]> ResolvePointerArray<T>(
            this Pointer<Pointer<T>[]> ptr,
            SerializerObject s,
            long count,
            PointerSize size = PointerSize.Pointer32,
            Pointer? anchor = null,
            bool allowInvalid = false,
            long? nullValue = null, 
            IStreamEncoder? encoder = null)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializePointerArray<T>(
                count: count,
                size: size,
                anchor: anchor,
                allowInvalid: allowInvalid,
                nullValue: nullValue), encoder);
        }

        public static Pointer<T[]> ResolveObjectArrayUntil<T>(
            this Pointer<T[]> ptr,
            SerializerObject s,
            Func<T, bool> conditionCheckFunc,
            Func<T>? getLastObjFunc = null,
            Action<T, int>? onPreSerialize = null, 
            IStreamEncoder? encoder = null)
            where T : BinarySerializable, new()
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return ptr.ResolveValue(s, PointerFunctions.SerializeObjectArrayUntil<T>(
                conditionCheckFunc: conditionCheckFunc,
                getLastObjFunc: getLastObjFunc,
                onPreSerialize: onPreSerialize), encoder);
        }

        public static Pointer<T[]> ResolveIntoArray<T>(this Pointer<T[]> ptr, SerializerObject s, long count, SerializeInto<T> serializeFunc, IStreamEncoder? encoder = null)
            where T : new()
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (serializeFunc == null)
                throw new ArgumentNullException(nameof(serializeFunc));

            return ptr.ResolveValue(s, PointerFunctions.SerializeIntoArray<T>(count, serializeFunc), encoder);
        }
        public static Pointer<T[]>[] ResolveIntoArray<T>(this Pointer<T[]>?[] ptrs, SerializerObject s, long count, SerializeInto<T> serializeFunc, IStreamEncoder? encoder = null)
            where T : new()
        {
            if (ptrs == null)
                throw new ArgumentNullException(nameof(ptrs));
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (serializeFunc == null)
                throw new ArgumentNullException(nameof(serializeFunc));

            for (int i = 0; i < ptrs.Length; i++)
            {
                Pointer<T[]>? ptr = ptrs[i];

                if (ptr == null)
                    ptrs[i] = new Pointer<T[]>();
                else
                    ptr.ResolveIntoArray(s, count, serializeFunc, encoder);
            }

            return ptrs!;
        }

        #endregion
    }
}