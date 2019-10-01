using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class Image
    {
        #region LoadPixelViewAs (IReadOnlyPixelView)

        public static unsafe Image<TPixelTo> LoadPixelViewAs<TPixelFrom, TPixelTo>(
            IReadOnlyPixelView<TPixelFrom> pixels, Rectangle sourceRectangle, ImagingConfig config)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentEmptyException(nameof(pixels));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Width, sourceRectangle.Height);
            if (pixels is IReadOnlyPixelMemory<TPixelTo> typeEqualMemory)
            {
                if (!typeEqualMemory.HasPadding() &&
                    sourceRectangle.Position == Point.Zero &&
                    sourceRectangle.Width == pixels.Width &&
                    sourceRectangle.Height == pixels.Height)
                {
                    typeEqualMemory.GetPixelSpan().CopyTo(image.GetPixelSpan());
                }
                else
                {
                    for (int y = 0; y < sourceRectangle.Height; y++)
                    {
                        var srcRow = typeEqualMemory.GetPixelRowSpan(sourceRectangle.Y + y);
                        srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                    }
                }
            }
            else if (pixels is IReadOnlyPixelRows<TPixelTo> typeEqualRows)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                        typeEqualRows.GetPixelRow(
                            sourceRectangle.X, y + sourceRectangle.Y, image.GetPixelRowSpan(y));
            }
            else if (pixels is IReadOnlyPixelBuffer<TPixelTo> typeEqualBuffer)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = typeEqualBuffer.GetPixelRowSpan(sourceRectangle.Y + y);
                    srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                }
            }
            else if (pixels is IReadOnlyPixelView<TPixelTo> typeEqualView)
            {
                Span<TPixelTo> dstSpan = image.GetPixelSpan();
                for (int y = 0; y < sourceRectangle.Height; y++)
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstSpan[x + y * sourceRectangle.Width] =
                            typeEqualView.GetPixel(x + sourceRectangle.X, y + sourceRectangle.Y);
            }
            else if (pixels is IReadOnlyPixelBuffer<TPixelFrom> pixelBuffer)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = pixelBuffer.GetPixelRowSpan(sourceRectangle.Y + y);
                    var dstRow = image.GetPixelRowSpan(y);
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstRow[x].FromVector4(srcRow[x + sourceRectangle.X].ToVector4());
                }
            }
            else
            {
                Span<TPixelTo> dstSpan = image.GetPixelSpan();
                for (int y = 0; y < sourceRectangle.Height; y++)
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstSpan[x + y * sourceRectangle.Width].FromVector4(
                            pixels.GetPixel(sourceRectangle.X + x, y + sourceRectangle.Height).ToVector4());
            }
            return image;
        }

        public static unsafe Image<TPixelTo> LoadPixelViewAs<TPixelFrom, TPixelTo>(
            IReadOnlyPixelView<TPixelFrom> pixels, Rectangle sourceRectangle)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelViewAs<TPixelFrom, TPixelTo>(pixels, sourceRectangle, ImagingConfig.Default);
        }

        public static unsafe Image<TPixelTo> LoadPixelViewAs<TPixelFrom, TPixelTo>(
            IReadOnlyPixelView<TPixelFrom> pixels)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelViewAs<TPixelFrom, TPixelTo>(pixels, pixels.GetBounds());
        }

        #endregion

        #region LoadPixelView (IReadOnlyPixelView)

        public static unsafe Image<TPixel> LoadPixelView<TPixel>(
            IReadOnlyPixelView<TPixel> pixels, Rectangle sourceRectangle, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelViewAs<TPixel, TPixel>(pixels, sourceRectangle, config);
        }

        public static unsafe Image<TPixel> LoadPixelView<TPixel>(
            IReadOnlyPixelView<TPixel> pixels, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelView(pixels, sourceRectangle, ImagingConfig.Default);
        }

        public static unsafe Image<TPixel> LoadPixelView<TPixel>(
            IReadOnlyPixelView<TPixel> pixels)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelView(pixels, pixels.GetBounds());
        }

        #endregion


        #region LoadPixels (ReadOnlySpan<byte>)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixelData.IsEmpty) throw new ArgumentEmptyException(nameof(pixelData));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixel>(sourceRectangle.Width, sourceRectangle.Height);
            fixed (byte* srcPixelPtr = &MemoryMarshal.GetReference(pixelData))
            fixed (TPixel* dstPixelPtr = &MemoryMarshal.GetReference(image.GetPixelSpan()))
            {
                byte* srcPtr = srcPixelPtr + sourceRectangle.X;
                byte* dstPtr = (byte*)(dstPixelPtr + sourceRectangle.X);
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    int rowOffset = (sourceRectangle.Y + y) * stride;
                    byte* srcRow = srcPtr + rowOffset;
                    byte* dstRow = dstPtr + rowOffset;
                    Buffer.MemoryCopy(srcRow, dstRow, stride, stride);
                }
            }
            return image;
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int stride)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel>(pixelData, sourceRectangle, stride, ImagingConfig.Default);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels<TPixel>(bytes, srcRect, width * sizeof(TPixel), ImagingConfig.Default);
        }

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, sourceRectangle)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            return LoadPixels<TPixel>(bytes, sourceRectangle, stride, config);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int stride)
            where TPixel : unmanaged, IPixel => 
            LoadPixels(pixelData, sourceRectangle, stride, ImagingConfig.Default);

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, width, height)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels(pixelData, srcRect, stride, config);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int stride)
            where TPixel : unmanaged, IPixel => 
            LoadPixels(pixelData, width, height, stride, ImagingConfig.Default);

        #endregion
    }
}
