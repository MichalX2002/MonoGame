using System;
using System.Threading;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Coders.Decoding
{
    /// <summary>
    /// Encapsulates decoding of images.
    /// </summary>
    public interface IImageDecoder : IImageCoder, IImagingConfigurable, IDisposable
    {
        /// <summary>
        /// Gets the options for this decoder.
        /// </summary>
        DecoderOptions DecoderOptions { get; }
        
        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        int FrameIndex { get; }

        /// <summary>
        /// Gets the current image buffer.
        /// </summary>
        Image? CurrentImage { get; }

        VectorType? SourcePixelType { get; } // TODO: move into CurrentImage metadata

        VectorType? TargetPixelType { get; set; }

        /// <summary>
        /// Decodes an image from the underlying stream. 
        /// </summary>
        void Decode(CancellationToken cancellationToken = default);
    }
}
