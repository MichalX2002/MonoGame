using MonoGame.Utilities.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    internal class Int32Comparer : IEqualityComparer<int>, IRefEqualityComparer<int>
    {
        public static readonly Int32Comparer Instance = new Int32Comparer();

        private Int32Comparer()
        {
        }

        public bool Equals(int x, int y)
        {
            return x.Equals(y);
        }

        public bool EqualsByRef(in int obj1, in int obj2)
        {
            return obj1 == obj2;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }

        public long GetLongHashCode(in int obj)
        {
            return obj;
        }
    }
}
