using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    internal struct Int32Comparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }
}
