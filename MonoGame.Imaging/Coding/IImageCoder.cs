
namespace MonoGame.Imaging
{
    /// <summary>
    /// Gives access to the <see cref="ImageFormat"/> used by image encoders and decoders.
    /// </summary>
    public interface IImageCoder
    {
        /// <summary>
        /// Gets the format associated with this coder.
        /// </summary>
        ImageFormat Format { get; }

        /// <summary>
        /// Gets whether this coder supports processing animated images.
        /// </summary>
        bool SupportsAnimation { get; }
    }
}
