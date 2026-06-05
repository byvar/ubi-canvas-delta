#nullable enable
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BinarySerializer
{
    /// <summary>
    /// Helper to cast to type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T">The type to cast to</typeparam>
    public static class CastTo<T>
    {
        /// <summary>
        /// Casts <see cref="S"/> to <see cref="T"/> without boxing the value
        /// </summary>
        /// <typeparam name="S">The type to cast from</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T From<S>(S s) => Cache<S>._caster(s);

        private static class Cache<S>
        {
            public static readonly Func<S, T> _caster = Get();

            private static Func<S, T> Get()
            {
                ParameterExpression p = Expression.Parameter(typeof(S));
                UnaryExpression c = Expression.ConvertChecked(p, typeof(T));
                return Expression.Lambda<Func<S, T>>(c, p).Compile();
            }
        }
    }
}