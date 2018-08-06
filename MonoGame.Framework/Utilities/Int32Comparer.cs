using MonoGame.Utilities.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    internal struct Int32Comparer : IEqualityComparer<int>, IRefEqualityComparer<int>
    {
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
