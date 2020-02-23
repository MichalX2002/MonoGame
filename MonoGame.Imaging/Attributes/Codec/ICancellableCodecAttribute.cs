using System.Threading;

namespace MonoGame.Imaging.Attributes.Codec
{
    /// <summary>
    /// The codec supports cancellation of coding operations by
    /// a <see cref="CancellationToken"/>.
    /// </summary>
    public interface ICancellableCodecAttribute : IImageCodecAttribute
    {
    }
}
