using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing an <see cref="ImagingConfig"/> and 
    /// pixels in the form of <see cref="IPixelRows"/>.
    /// </summary>
    public class PixelRowsContext : ReadOnlyPixelRowsContext, IPixelRows
    {
        public new IPixelRows Pixels => (IPixelRows)base.Pixels;

        public PixelRowsContext(ImagingConfig config, IPixelRows pixels) : base(config, pixels)
        {
        }

        public void SetPixelByteRow(int row, int column, ReadOnlySpan<byte> data)
        {
            Pixels.SetPixelByteRow(row, column, data);
        }
    }
}