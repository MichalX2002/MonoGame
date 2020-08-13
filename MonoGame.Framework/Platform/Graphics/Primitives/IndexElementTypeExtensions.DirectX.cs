using System;

namespace MonoGame.Framework.Graphics
{
    public static partial class IndexElementTypeExtensions
    {
        public static SharpDX.DXGI.Format ToDXElementType(this IndexElementType elementType)
        {
            switch (elementType)
            {
                case IndexElementType.Int16:
                    return SharpDX.DXGI.Format.R16_UInt;

                case IndexElementType.Int32:
                    return SharpDX.DXGI.Format.R32_UInt;

                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType));
            }
        }
    }
}
