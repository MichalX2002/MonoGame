
namespace MonoGame.Utilities.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongDoubleComparer : ILongEqualityComparer<double>
        {
            public bool Equals(double x, double y)
            {
                return x == y;
            }

            public int GetHashCode(double obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(double obj)
            {
                return GetLongDoubleHashCode(obj);
            }

            public static unsafe long GetLongDoubleHashCode(double d)
            {
                // Ensure that 0 and -0 have the same hash code
                if (d == 0)
                    return 0;

                return *(long*)&d;
            }
        }

        private class LongDecimalComparer : ILongEqualityComparer<decimal>
        {
            public bool Equals(decimal x, decimal y)
            {
                return x == y;
            }

            public int GetHashCode(decimal obj)
            {
                return obj.GetHashCode();
            }

            public long GetLongHashCode(decimal obj)
            {
                return LongDoubleComparer.GetLongDoubleHashCode((double)obj);
            }
        }
    }
}
