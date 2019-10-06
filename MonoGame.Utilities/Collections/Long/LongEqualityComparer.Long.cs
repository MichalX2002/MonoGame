
namespace MonoGame.Utilities.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongInt64Comparer : ILongEqualityComparer<long>
        {
            public bool Equals(long x, long y) => x == y;

            public int GetHashCode(long obj) => obj.GetHashCode();

            public long GetLongHashCode(long obj) => obj;
        }

        private class LongUInt64Comparer : ILongEqualityComparer<ulong>
        {
            public bool Equals(ulong x, ulong y) => x == y;

            public int GetHashCode(ulong obj) => obj.GetHashCode();

            public long GetLongHashCode(ulong obj) => unchecked((long)obj + long.MinValue);
        }
    }
}
