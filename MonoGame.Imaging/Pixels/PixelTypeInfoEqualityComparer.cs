using System.Collections.Generic;

namespace MonoGame.Imaging.Pixels
{
    public class PixelTypeInfoEqualityComparer : IEqualityComparer<PixelTypeInfo>
    {
        public static PixelTypeInfoEqualityComparer Instance { get; } = 
            new PixelTypeInfoEqualityComparer();

        private PixelTypeInfoEqualityComparer()
        {
        }

        public bool Equals(PixelTypeInfo x, PixelTypeInfo y)
        {
            return x.Type == y.Type
                && x.BitDepth == y.BitDepth;
        }

        public int GetHashCode(PixelTypeInfo obj)
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
