using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class PixelBufferContext<TPixel> : PixelBufferContext, IPixelBufferContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public new IPixelBuffer<TPixel> Pixels => (IPixelBuffer<TPixel>)base.Pixels;
        IReadOnlyPixelBuffer<TPixel> IReadOnlyPixelBufferContext<TPixel>.Pixels => Pixels;

        public PixelBufferContext(IImagingConfig imagingConfig, IPixelBuffer<TPixel> pixels) :
            base(imagingConfig, pixels)
        {
        }

        public void SetPixelRow(int x, int y, ReadOnlySpan<TPixel> data)
        {
            Pixels.SetPixelRow(x, y, data);
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            Pixels.GetPixelRow(x, y, destination);
        }

        public Span<TPixel> GetPixelRowSpan(int row)
        {
            return Pixels.GetPixelRowSpan(row);
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
        {
            return Pixels.GetPixelRowSpan(row);
        }
    }
}