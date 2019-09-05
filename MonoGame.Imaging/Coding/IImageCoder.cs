
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
        /// Gets whether this coder has implemented processing of animated images.
        /// <para>
        /// The format also needs to support animation.
        /// </para>
        /// </summary>
        bool ImplementsAnimation { get; }
    }
}
