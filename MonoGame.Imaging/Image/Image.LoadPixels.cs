using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private delegate void LoadPixelsDelegate(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination);

        private static ConcurrentDictionary<(Type, Type), LoadPixelsDelegate> _loadPixelRowsCache = 
            new ConcurrentDictionary<(Type, Type), LoadPixelsDelegate>();

        // TODO: add non-generic LoadPixels(Type pixelFrom, Type pixelTo, ...)

        public static Image LoadPixels(
            IReadOnlyPixelRows pixels, PixelTypeInfo imageType, Rectangle sourceRectangle)
        {
            var image = Image.Create(sourceRectangle.Size);
        }

        private static void LoadPixels(
            IReadOnlyPixelRows pixels, PixelTypeInfo imageType, Rectangle sourceRectangle, Image destination)
        {
        }

        #region LoadPixels<TPixel>(IReadOnlyPixelRows)

        public static Image<TPixel> LoadPixels<TPixel>(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            // TODO: test optimization: replacing Span<>.CopyTo with possibly faster memcpy

            if (pixels == null) throw new ArgumentEmptyException(nameof(pixels));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var dstImage = new Image<TPixel>(sourceRectangle.Width, sourceRectangle.Height);
            if (pixels is IReadOnlyPixelMemory<TPixel> typeEqualMemory)
            {
                if (typeEqualMemory.IsPixelContiguous &&
                    sourceRectangle.Position == Point.Zero &&
                    sourceRectangle.Width == pixels.Width &&
                    sourceRectangle.Height == pixels.Height)
                {
                    typeEqualMemory.GetPixelSpan().CopyTo(dstImage.GetPixelSpan());
                }
                else
                {
                    for (int y = 0; y < sourceRectangle.Height; y++)
                        typeEqualMemory.GetPixelRow(
                            sourceRectangle.X, sourceRectangle.Y + y, dstImage.GetPixelRowSpan(y));
                }
            }
            else if (pixels is IReadOnlyPixelBuffer<TPixel> typeEqualBuffer)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                    typeEqualBuffer.GetPixelRow(
                        sourceRectangle.X, sourceRectangle.Y + y, dstImage.GetPixelRowSpan(y));
            }
            else
            {
                _loadPixelRowsCache[]

                // TODO: make stack-allocated buffer
                var rowBuffer = new TPixelFrom[dstImage.Width];

                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    pixels.GetPixelByteRow(sourceRectangle.X, sourceRectangle.Y + y, rowBuffer);
                    var dstRow = dstImage.GetPixelRowSpan(y);
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstRow[x].FromScaledVector4(rowBuffer[x].ToScaledVector4());
                }
            }
            return dstImage;
        }

        public static Image<TPixel> LoadPixels<TPixel>(IReadOnlyPixelRows buffer)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel>(buffer, buffer.GetBounds());
        }

        #endregion

        #region LoadPixels(ReadOnlySpan)

        public static unsafe Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels,
            Rectangle sourceRectangle,
            int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (pixels.IsEmpty) throw new ArgumentEmptyException(nameof(pixels));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Width, sourceRectangle.Height);

            // TODO: check if the pixel span isn't padded and copy everything at once

            int srcRowBytes = sizeof(TPixelFrom) * sourceRectangle.Width;
            int srcStride = byteStride ?? srcRowBytes;
            int dstStride = image.ByteStride;

            fixed (TPixelFrom* srcPixelPtr = &MemoryMarshal.GetReference(pixels))
            fixed (TPixelTo* dstPixelPtr = &MemoryMarshal.GetReference(image.GetPixelSpan()))
            {
                byte* srcPtr = (byte*)srcPixelPtr + sourceRectangle.X * sizeof(TPixelFrom);
                byte* dstPtr = (byte*)dstPixelPtr + sourceRectangle.X * sizeof(TPixelTo);

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
                    for (int y = 0; y < sourceRectangle.Height; y++)
                    {
                        var srcRow = (TPixelFrom*)(srcPtr + (sourceRectangle.Y + y) * srcStride);
                        var dstRow = (TPixelTo*)(dstPtr + (sourceRectangle.Y + y) * dstStride);

                        for (int x = 0; x < sourceRectangle.Width; x++)
                            dstRow[x].FromScaledVector4(srcRow[x + sourceRectangle.X].ToScaledVector4());
                    }
                }
            }
            return image;
        }

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels, int width, int height, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels<TPixelFrom, TPixelTo>(pixels, srcRect, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
           ReadOnlySpan<TPixel> pixels, Rectangle sourceRectangle, int? byteStride = null)
           where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>(pixels, sourceRectangle, byteStride);
        }
        
        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixels, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels<TPixel, TPixel>(pixels, srcRect, byteStride);
        }

        #endregion

        #region LoadPixels(Span)

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> pixels, Rectangle sourceRectangle, int? byteStride)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixels<TPixelFrom, TPixelTo>(
                (ReadOnlySpan<TPixelFrom>)pixels, sourceRectangle, byteStride);
        }

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> pixels, int width, int height, int? byteStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixels<TPixelFrom, TPixelTo>(
                (ReadOnlySpan<TPixelFrom>)pixels, width, height, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            Span<TPixel> pixels, Rectangle sourceRectangle, int? byteStride)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>(
                (ReadOnlySpan<TPixel>)pixels, sourceRectangle, byteStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            Span<TPixel> pixels, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel, TPixel>(
                (ReadOnlySpan<TPixel>)pixels, width, height, byteStride);
        }

        #endregion
    }
}
