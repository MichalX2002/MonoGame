
namespace MonoGame.Utilities.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongInt64Comparer : ILongEqualityComparer<long>
        {
            public bool Equals(long x, long y)
            {
                return x == y;
            }

            public int GetHashCode(long obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(long obj)
            {
                return obj;
            }
        }

        private class LongUInt64Comparer : ILongEqualityComparer<ulong>
        {
            public bool Equals(ulong x, ulong y)
            {
                return x == y;
            }

            public int GetHashCode(ulong obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(ulong obj)
            {
                return unchecked((long)obj + long.MinValue);
            }
        }
    }
}
