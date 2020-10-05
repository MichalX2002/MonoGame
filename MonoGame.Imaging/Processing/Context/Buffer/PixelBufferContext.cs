using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class PixelBufferContext : ReadOnlyPixelBufferContext, IPixelBufferContext
    {
        public new IPixelBuffer Pixels => (IPixelBuffer)base.Pixels;
        IReadOnlyPixelBuffer IReadOnlyPixelBufferContext.Pixels => Pixels;

        public PixelBufferContext(IImagingConfig imagingConfig, IPixelBuffer pixels) :
            base(imagingConfig, pixels)
        {
        }

        public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
        {
            Pixels.SetPixelByteRow(x, y, data);
        }

        public new Span<byte> GetPixelByteRowSpan(int row)
        {
            return Pixels.GetPixelByteRowSpan(row);
        }
    }
}