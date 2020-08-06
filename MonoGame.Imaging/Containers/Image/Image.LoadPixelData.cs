using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region LoadPixelData<TFrom, TTo>(ReadOnlySpan<byte>)

        public static void LoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, Image<TPixelTo> destination, int? byteStride)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int srcOffsetX = sourceRectangle.X * Unsafe.SizeOf<TPixelFrom>();
            int srcByteStride = byteStride ?? (sourceRectangle.Width * Unsafe.SizeOf<TPixelFrom>());

            int dstByteStride = destination.ByteStride;
            var dstBytes = destination.GetPixelByteSpan();

            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                var srcByteRow = pixelData.Slice(srcOffsetX + (sourceRectangle.Y + y) * srcByteStride, srcByteStride);
                var dstByteRow = dstBytes.Slice(y * dstByteStride, dstByteStride);

                ConvertPixelData<TPixelFrom, TPixelTo>(srcByteRow, dstByteRow);
            }
        }

        #endregion

        #region LoadPixelData(FromType, ToType, ReadOnlySpan<byte>)

        public static Image LoadPixelData(
            VectorType sourceType,
            VectorType destinationType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride)
        {
            var loadDelegate = GetLoadPixelDataDelegate(sourceType, destinationType);
            var image = Create(destinationType, sourceRectangle.Size);
            try
            {
                loadDelegate.Invoke(pixelData, sourceRectangle, image, byteStride);
            }
            catch
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        #endregion

        #region LoadPixelData(Type, ReadOnlySpan<byte>)

        public static Image LoadPixelData(
            VectorType pixelType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride)
        {
            return LoadPixelData(pixelType, pixelType, pixelData, sourceRectangle, byteStride);
        }

        #endregion

        #region LoadPixelData<TTo>(FromType, ReadOnlySpan<byte>)

        public static Image<TPixelTo> LoadPixelData<TPixelTo>(
            VectorType sourceType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride)
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            var toType = VectorType.Get<TPixelTo>();
            var image = LoadPixelData(sourceType, toType, pixelData, sourceRectangle, byteStride);
            return (Image<TPixelTo>)image;
        }

        #endregion

        #region LoadPixelData<T>(ReadOnlySpan<byte>)

        public static Image<TPixel> LoadPixelData<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var type = VectorType.Get<TPixel>();
            var image = LoadPixelData(type, type, pixelData, sourceRectangle, byteStride);
            return (Image<TPixel>)image;
        }

        #endregion
    }
}
