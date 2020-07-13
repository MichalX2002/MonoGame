using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Coders
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

        /// <summary>
        /// Gets the default options for this coder.
        /// </summary>
        CoderOptions DefaultOptions { get; }
    }
}
