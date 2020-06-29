using System;
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
        private readonly Image.ConvertPixelsDelegate _convertPixels;

        public int Components { get; }
        public int Depth { get; }
        public int Width => _pixelRows.Size.Width;
        public int Height => _pixelRows.Size.Height;

        public PixelRowProvider(IReadOnlyPixelRows pixelRows, int components, int depth)
        {
            _pixelRows = pixelRows;
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
                _rowBuffer = new byte[pixelRows.Size.Width * pixelRows.ElementSize];
            }

            _convertPixels = Image.GetConvertPixelsDelegate(pixelRows.PixelType, resultType);
        }

        public unsafe void GetRow(int row, Span<byte> destination)
        {
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

        public unsafe void GetRow(int row, Span<float> destination)
        {
            throw new NotImplementedException();
        }

        public static VectorTypeInfo GetTypeByComp(int components, int depth)
        {
            Exception GetDepthException() => new ArgumentOutOfRangeException(nameof(depth));

            switch (components)
            {
                case 1:
                    if (depth == 8)
                        return VectorTypeInfo.Get<Alpha8>();
                    throw GetDepthException();

                case 2:
                    if (depth == 8)
                        return VectorTypeInfo.Get<GrayAlpha16>();
                    throw GetDepthException();

                case 3:
                    if (depth == 8)
                        return VectorTypeInfo.Get<Rgb24>();
                    throw GetDepthException();

                case 4:
                    if (depth == 8)
                        return VectorTypeInfo.Get<Color>();
                    throw GetDepthException();
            }
            throw new ArgumentOutOfRangeException(nameof(components));
        }
    }
}