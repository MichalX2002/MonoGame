using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class ReadOnlyPixelRowsContext<TPixel> : ReadOnlyPixelRowsContext, IReadOnlyPixelRowsContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public new IReadOnlyPixelRows<TPixel> Pixels => (IReadOnlyPixelRows<TPixel>)base.Pixels;

        public ReadOnlyPixelRowsContext(IImagingConfig imagingConfig, IReadOnlyPixelRows<TPixel> pixels) :
            base(imagingConfig, pixels)
        {
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            Pixels.GetPixelRow(x, y, destination);
        }
    }
}