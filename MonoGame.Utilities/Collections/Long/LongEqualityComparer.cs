using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        public static ILongEqualityComparer<T> Default { get; } = CreateComparer();

        private LongEqualityComparer()
        {
        }

        private static ILongEqualityComparer<T> CreateComparer()
        {
            Type type = typeof(T);
            if (type == typeof(string))
            {
                if (Environment.Is64BitProcess)
                    return (ILongEqualityComparer<T>)new LongStringComparer64();
                else
                    return (ILongEqualityComparer<T>)new LongStringComparer32();
            }

            if (type == typeof(long))
                return (ILongEqualityComparer<T>)new LongInt64Comparer();

            if (type == typeof(ulong))
                return (ILongEqualityComparer<T>)new LongUInt64Comparer();

            if (type == typeof(IntPtr))
                return (ILongEqualityComparer<T>)new LongIntPtrComparer();

            if (type == typeof(UIntPtr))
                return (ILongEqualityComparer<T>)new LongUIntPtrComparer();

            if (type == typeof(double))
                return (ILongEqualityComparer<T>)new LongDoubleComparer();

            if (type == typeof(decimal))
                return (ILongEqualityComparer<T>)new LongDecimalComparer();

            if (typeof(ILongHashable).IsAssignableFrom(type))
            {
                // LongHashableComparer is a nested class and takes T,
                // so we need to supply the generic type twice (for T and TLong)
                Type comparerType = typeof(LongHashableComparer<>).MakeGenericType(type, type); 
                return (ILongEqualityComparer<T>)Activator.CreateInstance(comparerType);
            }

            return new LongDefaultComparer();
        }

        private class LongHashableComparer<TLong> : ILongEqualityComparer<TLong>
            where TLong : ILongHashable
        {
            private static readonly IEqualityComparer<TLong> _internalComparer = EqualityComparer<TLong>.Default;

            public bool Equals(TLong x, TLong y) => _internalComparer.Equals(x, y);

            public int GetHashCode(TLong value) => _internalComparer.GetHashCode(value);

            public long GetLongHashCode(TLong value) => value.GetLongHashCode();
        }

        private class LongDefaultComparer : ILongEqualityComparer<T>
        {
            private static readonly IEqualityComparer<T> _internalComparer = EqualityComparer<T>.Default;

            public bool Equals(T x, T y) => _internalComparer.Equals(x, y);

            public int GetHashCode(T value) => _internalComparer.GetHashCode(value);

            public long GetLongHashCode(T value) => _internalComparer.GetHashCode(value);
        }
    }
}