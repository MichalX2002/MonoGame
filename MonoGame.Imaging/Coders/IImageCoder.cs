
namespace MonoGame.Imaging.Coders
{
    /// <summary>
    /// Base interface for image coders.
    /// </summary>
    public interface IImageCoder
    {
        /// <summary>
        /// Gets the format associated with this coder.
        /// </summary>
        ImageFormat Format { get; }

        /// <summary>
        /// Gets the options for this coder.
        /// </summary>
        CoderOptions CoderOptions { get; }
    }
}
