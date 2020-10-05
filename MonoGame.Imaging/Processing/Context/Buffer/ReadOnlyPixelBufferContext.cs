using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class ReadOnlyPixelBufferContext : ReadOnlyPixelRowsContext, IReadOnlyPixelBufferContext
    {
        public new IReadOnlyPixelBuffer Pixels => (IReadOnlyPixelBuffer)base.Pixels;

        public ReadOnlyPixelBufferContext(
            IImagingConfig imagingConfig, IReadOnlyPixelBuffer pixels) : base(imagingConfig, pixels)
        {
        }

        public ReadOnlySpan<byte> GetPixelByteRowSpan(int row)
        {
            return Pixels.GetPixelByteRowSpan(row);
        }
    }
}