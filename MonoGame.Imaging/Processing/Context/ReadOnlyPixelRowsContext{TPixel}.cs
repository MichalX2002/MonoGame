using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public readonly struct ReadOnlyPixelRowsContext<TPixel> : IReadOnlyPixelRows<TPixel>, IImagingConfigProvider
        where TPixel : unmanaged, IPixel
    {
        public ImagingConfig ImagingConfig { get; }
        public IReadOnlyPixelRows<TPixel> Pixels { get; }

        public bool IsEmpty => Pixels == null;

        public int Count => Pixels.Count;
        public int ElementSize => Pixels.ElementSize;

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;
        public VectorTypeInfo PixelType => Pixels.PixelType;

        public ReadOnlyPixelRowsContext(ImagingConfig imagingConfig, IReadOnlyPixelRows<TPixel> pixels)
        {
            ImagingConfig = imagingConfig ?? throw new ArgumentNullException(nameof(imagingConfig));
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            Pixels.GetPixelByteRow(x, y, destination);
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            Pixels.GetPixelRow(x, y, destination);
        }

        public void Dispose()
        {
        }
    }
}