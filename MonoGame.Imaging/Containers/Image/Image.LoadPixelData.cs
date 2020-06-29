using System;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region LoadPixelData(FromType, ToType, ReadOnlySpan<byte>)

        public static Image LoadPixelData(
            VectorTypeInfo sourceType,
            VectorTypeInfo destinationType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride = null)
        {
            var loadDelegate = GetLoadPixelSpanDelegate(sourceType, destinationType);
            var image = Create(destinationType, sourceRectangle.Size);
            try
            {
                loadDelegate.Invoke(pixelData, sourceRectangle, byteStride, image);
            }
            catch
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        public static Image LoadPixelData(
            VectorTypeInfo sourceType,
            VectorTypeInfo destinationType,
            ReadOnlySpan<byte> pixelData,
            Size size,
            int? byteStride = null)
        {
            return LoadPixelData(
                sourceType, destinationType, pixelData, new Rectangle(size), byteStride);
        }

        #endregion

        #region LoadPixelData(Type, ReadOnlySpan<byte>)

        public static Image LoadPixelData(
            VectorTypeInfo pixelType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride = null)
        {
            return LoadPixelData(pixelType, pixelType, pixelData, sourceRectangle, byteStride);
        }

        public static Image LoadPixelData(
            VectorTypeInfo pixelType, ReadOnlySpan<byte> pixelData, Size size, int? byteStride = null)
        {
            return LoadPixelData(pixelType, pixelData, new Rectangle(size), byteStride);
        }

        #endregion

        #region LoadPixelData<TFrom, TTo>(ReadOnlySpan<byte>)

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            var fromType = VectorTypeInfo.Get<TPixelFrom>();
            return LoadPixelData<TPixelTo>(fromType, pixelData, sourceRectangle, byteStride);
        }

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData, Size size, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelFrom, TPixelTo>(pixelData, new Rectangle(size), byteStride);
        }

        #endregion

        #region LoadPixelData<TTo>(FromType, ReadOnlySpan<byte>)

        public static Image<TPixelTo> LoadPixelData<TPixelTo>(
            VectorTypeInfo sourceType,
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride = null)
            where TPixelTo : unmanaged, IPixel
        {
            var toType = VectorTypeInfo.Get<TPixelTo>();
            var image = LoadPixelData(sourceType, toType, pixelData, sourceRectangle, byteStride);
            return (Image<TPixelTo>)image;
        }

        public static Image<TPixelTo> LoadPixelData<TPixelTo>(
            VectorTypeInfo sourceType, ReadOnlySpan<byte> pixelData, Size size, int? byteStride = null)
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelTo>(sourceType, pixelData, new Rectangle(size), byteStride);
        }

        #endregion

        #region LoadPixelData<T>(ReadOnlySpan<byte>)

        public static Image<TPixel> LoadPixelData<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelData<TPixel, TPixel>(pixelData, sourceRectangle, byteStride);
        }

        public static Image<TPixel> LoadPixelData<TPixel>(
            ReadOnlySpan<byte> pixelData, Size size, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelData<TPixel, TPixel>(pixelData, new Rectangle(size), byteStride);
        }

        #endregion
    }
}
