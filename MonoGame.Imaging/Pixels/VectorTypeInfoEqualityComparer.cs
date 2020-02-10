using System.Collections.Generic;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    public class VectorTypeInfoEqualityComparer : IEqualityComparer<VectorTypeInfo>
    {
        public static VectorTypeInfoEqualityComparer Instance { get; } = 
            new VectorTypeInfoEqualityComparer();

        private VectorTypeInfoEqualityComparer()
        {
        }

        public bool Equals(VectorTypeInfo x, VectorTypeInfo y)
        {
            return x.Type == y.Type
                && x.BitDepth == y.BitDepth;
        }

        public int GetHashCode(VectorTypeInfo obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Type.GetHashCode();
                hash = hash * 23 + obj.BitDepth.GetHashCode();
                return hash;
            }
        }
    }
}
