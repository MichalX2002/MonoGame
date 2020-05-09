using System;
using MonoGame.Framework;
using MonoGame.Framework.Vector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public class ReadOnlyPixelRowsContext : IReadOnlyPixelRowsContext
    {
        public IImagingConfig Config { get; }
        public IReadOnlyPixelRows Pixels { get; }

        public int Length => Pixels.Length;
        public int ElementSize => Pixels.ElementSize;

        public VectorTypeInfo PixelType => Pixels.PixelType;
        public Size Size => Pixels.Size;

        public ReadOnlyPixelRowsContext(IImagingConfig imagingConfig, IReadOnlyPixelRows pixels)
        {
            Config = imagingConfig ?? throw new ArgumentNullException(nameof(imagingConfig));
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            Pixels.GetPixelByteRow(x, y, destination);
        }

        public virtual void Dispose()
        {
        }
    }
}