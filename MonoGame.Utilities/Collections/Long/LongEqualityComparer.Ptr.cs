using System;

namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongIntPtrComparer : ILongEqualityComparer<IntPtr>
        {
            public bool Equals(IntPtr x, IntPtr y) => x == y;

            public int GetHashCode(IntPtr obj) => obj.GetHashCode();

            public long GetLongHashCode(IntPtr obj) => obj.ToInt64();
        }

        private class LongUIntPtrComparer : ILongEqualityComparer<UIntPtr>
        {
            public bool Equals(UIntPtr x, UIntPtr y) => x == y;

            public int GetHashCode(UIntPtr obj) => obj.GetHashCode();

            public long GetLongHashCode(UIntPtr obj) => unchecked((long)obj.ToUInt64() + long.MinValue);
        }
    }
}
