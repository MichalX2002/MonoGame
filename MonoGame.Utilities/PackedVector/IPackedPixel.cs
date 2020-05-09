
namespace MonoGame.Framework.PackedVector
{
    public interface IPackedPixel<TSelf, TPacked> : IPixel<TSelf>, IPackedVector<TSelf, TPacked>
        where TSelf : IPackedPixel<TSelf, TPacked>
    {
    }
}
