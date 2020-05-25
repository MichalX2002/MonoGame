
using System;

namespace MonoGame.Framework.Vector
{
    [CLSCompliant(false)]
    public interface IPackedPixel<TSelf, TPacked> : IPixel<TSelf>, IPackedVector<TSelf, TPacked>
        where TSelf : IPackedPixel<TSelf, TPacked>
    {
    }
}
