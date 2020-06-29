using System;

namespace MonoGame.Framework.Vectors
{
    [CLSCompliant(false)]
    public interface IPackedPixel<TSelf, TPacked> : IPackedVector<TSelf, TPacked>, IPixel<TSelf>
        where TSelf : IPackedPixel<TSelf, TPacked>
    {
    }
}
