using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class PixelRowsContext<TPixel> : PixelRowsContext, IPixelRowsContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public new IPixelRows<TPixel> Pixels => (IPixelRows<TPixel>)base.Pixels;
        IReadOnlyPixelRows<TPixel> IReadOnlyPixelRowsContext<TPixel>.Pixels => Pixels;

        public PixelRowsContext(IImagingConfig imagingConfig, IPixelRows<TPixel> pixels) :
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
    }
}