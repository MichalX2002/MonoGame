using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Base interface for image codecs.
    /// </summary>
    public interface IImageCodec : IImageCodecAttribute
    {
        /// <summary>
        /// Gets the format associated with this codec.
        /// </summary>
        ImageFormat Format { get; }
    }
}
