using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public readonly struct ReadOnlyPixelRowsContext : IReadOnlyPixelRows, IImagingConfigProvider
    {
        public ImagingConfig ImagingConfig { get; }
        public IReadOnlyPixelRows Pixels { get; }

        public bool IsEmpty => Pixels == null;

        public int Count => Pixels.Count;
        public int ElementSize => Pixels.ElementSize;

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;
        public PixelTypeInfo PixelType => Pixels.PixelType;

        public ReadOnlyPixelRowsContext(ImagingConfig imagingConfig, IReadOnlyPixelRows pixels)
        {
            ImagingConfig = imagingConfig ?? throw new ArgumentNullException(nameof(imagingConfig));
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            Pixels.GetPixelByteRow(x, y, destination);
        }

        public void Dispose()
        {
        }
    }
}