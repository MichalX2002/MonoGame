using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class PixelRowsContext : ReadOnlyPixelRowsContext, IPixelRowsContext
    {
        public new IPixelRows Pixels => (IPixelRows)base.Pixels;
        IReadOnlyPixelRows IReadOnlyPixelRowsContext.Pixels => Pixels;

        public PixelRowsContext(IImagingConfig imagingConfig, IPixelRows pixels) :
            base(imagingConfig, pixels)
        {
        }

        public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
        {
            Pixels.SetPixelByteRow(x, y, data);
        }
    }
}