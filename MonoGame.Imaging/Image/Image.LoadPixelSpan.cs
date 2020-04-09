using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private static unsafe void LoadPixelSpan<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels,
            Rectangle sourceRectangle,
            int? byteStride,
            Image<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            // TODO: check if the pixel span isn't padded and copy everything at once

            int srcRowBytes = sizeof(TPixelFrom) * sourceRectangle.Width;
            int srcStride = byteStride ?? srcRowBytes;
            int dstStride = destination.ByteStride;

            fixed (TPixelFrom* srcPixelPtr = pixels)
            fixed (TPixelTo* dstPixelPtr = destination.GetPixelSpan())
            {
                byte* srcPtr = (byte*)(srcPixelPtr + sourceRectangle.X);
                byte* dstPtr = (byte*)(dstPixelPtr + sourceRectangle.X);

                if (typeof(TPixelFrom) == typeof(TPixelTo))
                {
                    for (int y = 0; y < sourceRectangle.Height; y++)
                    {
                        byte* srcRow = srcPtr + (sourceRectangle.Y + y) * srcStride;
                        byte* dstRow = dstPtr + (sourceRectangle.Y + y) * dstStride;
                        Buffer.MemoryCopy(srcRow, dstRow, dstStride, srcRowBytes);
                    }
                }
                else
                {
                    if (typeof(TPixelFrom) == typeof(Color))
                    {
                        for (int y = 0; y < sourceRectangle.Height; y++)
                        {
                            var srcRow = (Color*)(srcPtr + (sourceRectangle.Y + y) * srcStride);
                            var dstRow = (TPixelTo*)(dstPtr + (sourceRectangle.Y + y) * dstStride);

                            for (int x = 0; x < sourceRectangle.Width; x++)
                                dstRow[x].FromColor(srcRow[x]);
                        }
                    }
                    else
                    {
                        for (int y = 0; y < sourceRectangle.Height; y++)
                        {
                            var srcRow = (TPixelFrom*)(srcPtr + (sourceRectangle.Y + y) * srcStride);
                            var dstRow = (TPixelTo*)(dstPtr + (sourceRectangle.Y + y) * dstStride);

                            for (int x = 0; x < sourceRectangle.Width; x++)
                            {
                                srcRow[x].ToScaledVector4(out var vector);
                                dstRow[x].FromScaledVector4(vector);
                            }
                        }
                    }
                }
            }
        }

        #region LoadPixels(ReadOnlySpan)

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels,
            Rectangle sourceRectangle,
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (pixels.IsEmpty) throw new ArgumentEmptyException(nameof(pixels));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Size);
            try
            {
                LoadPixelSpan(pixels, sourceRectangle, byteStride, image);
            }
            catch
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels, Size size, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixels<TPixelFrom, TPixelTo>(pixels, new Rectangle(size), byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
           ReadOnlySpan<TPixel> pixels, Rectangle sourceRectangle, int? byteStride = null)
           where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>(pixels, sourceRectangle, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixels, Size size, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>(pixels, new Rectangle(size), byteStride);
        }

        #endregion

        #region LoadPixels(Span)

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> pixels, Rectangle sourceRectangle, int? byteStride)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixels<TPixelFrom, TPixelTo>((ReadOnlySpan<TPixelFrom>)pixels, sourceRectangle, byteStride);
        }

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> pixels, Size size, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixels<TPixelFrom, TPixelTo>((ReadOnlySpan<TPixelFrom>)pixels, size, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            Span<TPixel> pixels, Rectangle sourceRectangle, int? byteStride)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>((ReadOnlySpan<TPixel>)pixels, sourceRectangle, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            Span<TPixel> pixels, Size size, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>((ReadOnlySpan<TPixel>)pixels, size, byteStride);
        }

        #endregion
    }
}
