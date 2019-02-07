
using System;

namespace MonoGame.Utilities.Collections
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
                return LongHashableComparer.Instance;

            return LongDefaultComparer.Instance;
        }

        private class LongHashableComparer : ILongEqualityComparer<T>
        {
            public static readonly LongHashableComparer Instance = new LongHashableComparer();

            public bool Equals(T x, T y)
            {
                return Default.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return Default.GetHashCode(obj);
            }

            public long GetLongHashCode(T obj)
            {
                if (obj is ILongHashable hashable)
                    return hashable.GetLongHashCode();
                throw new InvalidCastException();
            }
        }

        private class LongDefaultComparer : ILongEqualityComparer<T>
        {
            public static readonly LongDefaultComparer Instance = new LongDefaultComparer();

            public bool Equals(T x, T y)
            {
                return Default.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return Default.GetHashCode(obj);
            }

            public long GetLongHashCode(T obj)
            {
                return GetHashCode(obj);
            }
        }
    }
}