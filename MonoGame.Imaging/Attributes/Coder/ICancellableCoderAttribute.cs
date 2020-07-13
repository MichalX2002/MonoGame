using System.Threading;

namespace MonoGame.Imaging.Attributes.Coder
{
    /// <summary>
    /// The coder supports cancellation of coding operations by
    /// a <see cref="CancellationToken"/>.
    /// </summary>
    public interface ICancellableCoderAttribute : IImageCoderAttribute
    {
    }
}
