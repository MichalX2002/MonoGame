
namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Base interface for image coders.
    /// </summary>
    public interface IImageCoder : IImageCoderAttribute
    {
        /// <summary>
        /// Gets the format associated with this coder.
        /// </summary>
        ImageFormat Format { get; }
    }
}
