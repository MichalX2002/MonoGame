using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public readonly struct PixelRowsContext : IPixelRows, IImagingConfigProvider
    {
        public ImagingConfig ImagingConfig { get; }
        public IPixelRows Pixels { get; }

        public bool IsEmpty => Pixels == null;

        public int Count => Pixels.Count;
        public int ElementSize => Pixels.ElementSize;

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;
        public VectorTypeInfo PixelType => Pixels.PixelType;

        public PixelRowsContext(ImagingConfig imagingConfig, IPixelRows pixels)
        {
            ImagingConfig = imagingConfig ?? throw new ArgumentNullException(nameof(imagingConfig));
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
        {
            Pixels.SetPixelByteRow(x, y, data);
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