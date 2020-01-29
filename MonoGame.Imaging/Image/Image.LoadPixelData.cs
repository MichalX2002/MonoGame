using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region LoadPixelData(ReadOnlySpan<byte>)

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData, 
            Rectangle sourceRectangle, 
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelFrom, TPixelTo>(pixelData, sourceRectangle, byteStride);
        }

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData, 
            int width, 
            int height,
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelFrom, TPixelTo>(
                pixelData, new Rectangle(0, 0, width, height), byteStride);
        }

        #endregion

        #region LoadPixelData(Span<byte>)

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            Span<byte> pixelData,
            Rectangle sourceRectangle,
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelFrom, TPixelTo>(pixelData, sourceRectangle, byteStride);
        }

        public static Image<TPixelTo> LoadPixelData<TPixelFrom, TPixelTo>(
            Span<byte> pixelData,
            int width,
            int height,
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelData<TPixelFrom, TPixelTo>(
                pixelData, new Rectangle(0, 0, width, height), byteStride);
        }

        #endregion
    }
}
