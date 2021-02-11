using System;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct PixelRowProvider : IPixelRowProvider
    {
        private readonly IReadOnlyPixelRows _pixelRows;
        private readonly IReadOnlyPixelBuffer? _pixelBuffer;
        private readonly byte[]? _rowBuffer;
        private readonly Image.ConvertPixelDataDelegate _convertPixels;

        public CancellationToken CancellationToken { get; }
        public int Components { get; }
        public int Depth { get; }

        public int Width => _pixelRows.Width;
        public int Height => _pixelRows.Height;

        public PixelRowProvider(IReadOnlyPixelRows pixelRows, int components, int depth, CancellationToken cancellationToken)
        {
            _pixelRows = pixelRows ?? throw new ArgumentNullException(nameof(pixelRows));
            CancellationToken = cancellationToken;
            Components = components;
            Depth = depth;

            // call early to check args
            var resultType = GetTypeByComp(Components, Depth);

            if (_pixelRows is IReadOnlyPixelBuffer buffer)
            {
                _pixelBuffer = buffer;
                _rowBuffer = null;
            }
            else
            {
                _pixelBuffer = null;
                _rowBuffer = new byte[pixelRows.Width * pixelRows.ElementSize];
            }

            _convertPixels = Image.GetConvertPixelsDelegate(pixelRows.PixelType, resultType);
        }

        public void GetByteRow(int row, Span<byte> destination)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (_pixelBuffer != null)
            {
                var rowSpan = _pixelBuffer.GetPixelByteRowSpan(row);
                _convertPixels.Invoke(rowSpan, destination);
            }
            else
            {
                _pixelRows.GetPixelByteRow(0, row, _rowBuffer);
                _convertPixels.Invoke(_rowBuffer, destination);
            }
        }

        public void GetFloatRow(int row, Span<float> destination)
        {
            CancellationToken.ThrowIfCancellationRequested();

            throw new NotImplementedException();
        }

        public static VectorType GetTypeByComp(int components, int depth)
        {
            Exception GetDepthException() => new ArgumentOutOfRangeException(nameof(depth));

            switch (components)
            {
                case 1:
                    if (depth == 8)
                        return VectorType.Get<Alpha8>();
                    throw GetDepthException();

                case 2:
                    if (depth == 8)
                        return VectorType.Get<GrayAlpha16>();
                    throw GetDepthException();

                case 3:
                    if (depth == 8)
                        return VectorType.Get<Rgb24>();
                    throw GetDepthException();

                case 4:
                    if (depth == 8)
                        return VectorType.Get<Color>();
                    throw GetDepthException();
            }
            throw new ArgumentOutOfRangeException(nameof(components));
        }
    }
}