using System;

namespace MonoGame.Utilities.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongIntPtrComparer : ILongEqualityComparer<IntPtr>
        {
            public bool Equals(IntPtr x, IntPtr y)
            {
                return x == y;
            }

            public int GetHashCode(IntPtr obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(IntPtr obj)
            {
                return obj.ToInt64();
            }
        }

        private class LongUIntPtrComparer : ILongEqualityComparer<UIntPtr>
        {
            public bool Equals(UIntPtr x, UIntPtr y)
            {
                return x == y;
            }

            public int GetHashCode(UIntPtr obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(UIntPtr obj)
            {
                return unchecked((long)obj.ToUInt64() + long.MinValue);
            }
        }
    }
}
