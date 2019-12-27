
namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongDecimalComparer : ILongEqualityComparer<decimal>
        {
            public bool Equals(decimal x, decimal y) => x == y;

            public int GetHashCode(decimal obj) => obj.GetHashCode();

            public long GetLongHashCode(decimal obj) => LongDoubleComparer.GetLongDoubleHashCode((double)obj);
        }
    }
}
