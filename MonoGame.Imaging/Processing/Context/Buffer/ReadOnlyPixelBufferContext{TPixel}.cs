using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class ReadOnlyPixelBufferContext<TPixel> : ReadOnlyPixelBufferContext, IReadOnlyPixelBufferContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public new IReadOnlyPixelBuffer<TPixel> Pixels => (IReadOnlyPixelBuffer<TPixel>)base.Pixels;

        public ReadOnlyPixelBufferContext(IImagingConfig imagingConfig, IReadOnlyPixelBuffer<TPixel> pixels) :
            base(imagingConfig, pixels)
        {
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            Pixels.GetPixelRow(x, y, destination);
        }

        public ReadOnlySpan<TPixel> GetPixelRowSpan(int row)
        {
            return Pixels.GetPixelRowSpan(row);
        }
    }
}