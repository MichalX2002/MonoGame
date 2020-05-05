using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Codecs
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

        /// <summary>
        /// Gets the default options for this codec.
        /// </summary>
        CodecOptions DefaultOptions { get; }
    }
}
