using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing an <see cref="ImagingConfig"/> and 
    /// pixels in the form of <see cref="IReadOnlyPixelRows"/>.
    /// </summary>
    public class ReadOnlyPixelRowsContext : IReadOnlyPixelRows
    {
        private IReadOnlyPixelRows _pixels;

        public bool IsDisposed { get; private set; }
        public ImagingConfig Config { get; }

        public IReadOnlyPixelRows Pixels
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return _pixels;
            }
        }

        public int Count => Pixels.Count;
        public int ElementSize => Pixels.ElementSize;

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;
        public PixelTypeInfo PixelType => Pixels.PixelType;

        public ReadOnlyPixelRowsContext(ImagingConfig config, IReadOnlyPixelRows pixels)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            Pixels.GetPixelByteRow(x, y, destination);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ReadOnlyPixelRowsContext()
        {
            Dispose(false);
        }
    }
}
