using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Collections
{
    public abstract partial class LongEqualityComparer<T> : EqualityComparer<T>, ILongEqualityComparer<T>
    {
        public static new LongEqualityComparer<T> Default { get; } = CreateComparer();

        public LongEqualityComparer()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals([AllowNull] T x, [AllowNull] T y) => EqualityComparer<T>.Default.Equals(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode([DisallowNull] T value) => EqualityComparer<T>.Default.GetHashCode(value);

        public abstract long GetLongHashCode([DisallowNull] T value);

        private static LongEqualityComparer<T> CreateComparer()
        {
            if (typeof(T).IsGenericTypeDefinition &&
                typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T))!;
                Type comparerType = typeof(LongNullableComparer<>).MakeGenericType(underlyingType);
                return (LongEqualityComparer<T>)Activator.CreateInstance(comparerType)!;
            }

            if (typeof(T) == typeof(string))
                // LongStringComparer is "randomized" by default
                return (LongEqualityComparer<T>)(object)new LongStringComparer();

            if (typeof(T) == typeof(long))
                return (LongEqualityComparer<T>)(object)new LongInt64Comparer();

            if (typeof(T) == typeof(ulong))
                return (LongEqualityComparer<T>)(object)new LongUInt64Comparer();

            if (typeof(T) == typeof(IntPtr))
                return (LongEqualityComparer<T>)(object)new LongIntPtrComparer();

            if (typeof(T) == typeof(UIntPtr))
                return (LongEqualityComparer<T>)(object)new LongUIntPtrComparer();

            if (typeof(T) == typeof(double))
                return (LongEqualityComparer<T>)(object)new LongDoubleComparer();

            if (typeof(T) == typeof(decimal))
                return (LongEqualityComparer<T>)(object)new LongDecimalComparer();

            if (typeof(ILongHashable).IsAssignableFrom(typeof(T)))
            {
                Type comparerType = typeof(LongHashableComparer<>).MakeGenericType(typeof(T));
                return (LongEqualityComparer<T>)Activator.CreateInstance(comparerType)!;
            }

            return new LongGenericComparer<T>();
        }
    }
}