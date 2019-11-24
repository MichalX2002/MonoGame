
namespace MonoGame.Imaging
{
    /// <summary>
    /// Base interface for image coders.
    /// Exposes an <see cref="ImageFormat"/> and whether cancellation is supported.
    /// </summary>
    public interface IImageCoder
    {
        /// <summary>
        /// Gets the format associated with this coder.
        /// </summary>
        ImageFormat Format { get; }

        /// <summary>
        /// Gets whether a coding operation can be cancelled at the
        /// request of the supplied <see cref="System.Threading.CancellationToken"/>.
        /// </summary>
        bool SupportsCancellation { get; }
    }
}
