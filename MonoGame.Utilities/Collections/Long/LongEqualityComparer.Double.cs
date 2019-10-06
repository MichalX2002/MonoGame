
namespace MonoGame.Utilities.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private class LongDoubleComparer : ILongEqualityComparer<double>
        {
            public bool Equals(double x, double y) => x == y;

            public int GetHashCode(double obj) => obj.GetHashCode();

            public long GetLongHashCode(double obj) => GetLongDoubleHashCode(obj);

            public static unsafe long GetLongDoubleHashCode(double d)
            {
                // Ensure that 0 and -0 have the same hash code
                if (d == 0)
                    return 0;

                return *(long*)&d;
            }
        }
    }
}
