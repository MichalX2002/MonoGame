
namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Encapsulates decoding of images.
    /// </summary>
    public interface IImageDecoder : IImageCodec
    {
        /// <summary>
        /// Gets the default options for this decoder.
        /// </summary>
        new DecoderOptions DefaultOptions { get; }

        /// <summary>
        /// Creates a state that can be used to decode images.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The state used to decode images.</returns>
        ImageDecoderState CreateState(ImagingConfig config, ImageReadStream stream);

        /// <summary>
        /// Decodes an image from the decoder state stream. 
        /// </summary>
        /// <param name="decoderState">The state to decode from.</param>
        bool Decode(ImageDecoderState decoderState);
    }
}
